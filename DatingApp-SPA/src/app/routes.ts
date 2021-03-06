import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { MemberEditResolver } from './_resolvers/member-edit.resolver';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { AuthGuard } from './_guards/auth.guard';
import { HomeComponent } from './home/home.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { ListsComponent } from './lists/lists.component';
import { Routes } from '@angular/router';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';

export const appRoute: Routes = [
    { path: '', component: HomeComponent },
    {
        path: '',
        canActivate: [AuthGuard],
        children: [
            {
                path: 'members', component: MemberListComponent, resolve: {
                    users: MemberListResolver
                }
            },
            {
                path: 'members/:id', component: MemberDetailComponent, resolve: {
                    user: MemberDetailResolver
                }
            },
            {
                path: 'member/edit', component: MemberEditComponent, resolve: {
                    user: MemberEditResolver
                },
                canDeactivate: [PreventUnsavedChanges]
            },
            { path: 'messages', component: MessagesComponent },
            { path: 'lists', component: ListsComponent }]
    },
    { path: '**', redirectTo: '', pathMatch: 'full' }
];
