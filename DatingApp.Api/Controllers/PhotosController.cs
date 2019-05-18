using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.Api.Data;
using DatingApp.Api.DTOS;
using DatingApp.Api.Helpers;
using DatingApp.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.Api.Controllers {
    [Authorize]
    [Route("api/users/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase {
        private readonly IDatingRepository datingRepo;
        private readonly IOptions<CloudinarySettings> cloudinaryConfig;
        private readonly Cloudinary cloudinary;
        private readonly IMapper mapper;

        public PhotosController(IDatingRepository datingRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) {
            this.cloudinaryConfig = cloudinaryConfig;
            this.datingRepo = datingRepo;
            this.mapper = mapper;
            Account acc = new Account(
                this.cloudinaryConfig.Value.CloudName,
                this.cloudinaryConfig.Value.ApiKey,
                this.cloudinaryConfig.Value.ApiSecret
            );
            this.cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) {
            var photoFromRepo = await datingRepo.GetPhoto(id);
            var photo = mapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser([FromForm] PhotoForCreationDTO photoForCreationDTO) {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await datingRepo.GetUser(currentUserId);

            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();

            if (file != null) {
                using(var stream = file.OpenReadStream()) {

                var uploadParams = new ImageUploadParams() {
                File = new FileDescription(file.Name, stream),
                Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };
                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId.ToString();

            var photo = mapper.Map<Photo>(photoForCreationDTO);
            if (!userFromRepo.Photos.Any(u => u.IsMain)) {
                photo.IsMain = true;
            }
            userFromRepo.Photos.Add(photo);
            if (!await datingRepo.SaveAll()) {
                return BadRequest("Could not add the photo");
            }
            var photoToReturn = mapper.Map<PhotoForReturnDTO>(photo);
            return CreatedAtRoute("GetPhoto", new { id = photo.Id }, photoToReturn);
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int id) {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await datingRepo.GetUser(currentUserId);
            if (!userFromRepo.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }
            var photoFromRepo = await datingRepo.GetPhoto(id);
            if (photoFromRepo.IsMain) {
                return BadRequest("This is already the main photo");
            }
            var currentMainPhoto = await datingRepo.GetMainPhotoFromUser(currentUserId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (!await datingRepo.SaveAll()) {
                return BadRequest("Could not set photo to main");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int id) {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await datingRepo.GetUser(currentUserId);
            if (!userFromRepo.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }
            var photoFromRepo = await datingRepo.GetPhoto(id);
            if (photoFromRepo.IsMain) {
                return BadRequest("Can't delete main photo");
            }
            if (photoFromRepo.PublicId != null) {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = cloudinary.Destroy(deleteParams);
                if (result.Result != "ok") {
                    return BadRequest("Failed to delete the photo");
                }
            }
            datingRepo.Delete(photoFromRepo);
            if (!await datingRepo.SaveAll()) {
                return BadRequest("Could not delete photo");
            }
            return Ok();
        }
    }
}