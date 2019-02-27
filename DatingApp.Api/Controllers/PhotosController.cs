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
        private readonly IDatingRepository _datingRepo;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;

        public PhotosController(IDatingRepository datingRepo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) {
            _cloudinaryConfig = cloudinaryConfig;
            _datingRepo = datingRepo;
            _mapper = mapper;
            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary = new Cloudinary(acc);
        }
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id){
            var photoFromRepo = await _datingRepo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);
            return Ok(photo);
        }
        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser([FromForm]PhotoForCreationDTO photoForCreationDTO) {
            int currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _datingRepo.GetUser(currentUserId);

            var file = photoForCreationDTO.File;
            var uploadResult = new ImageUploadResult();

            if (file != null) {
                using(var stream = file.OpenReadStream()) {

                    var uploadParams = new ImageUploadParams() {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")

                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoForCreationDTO.Url = uploadResult.Uri.ToString();
            photoForCreationDTO.PublicId = uploadResult.PublicId.ToString();

            var photo = _mapper.Map<Photo>(photoForCreationDTO);
            if(!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }
            userFromRepo.Photos.Add(photo);
            if (!await _datingRepo.SaveAll()) {
                return BadRequest("Could not add the photo");
            }
            var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
            return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
        }
    }
}