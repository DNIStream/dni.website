import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { HomeComponent } from 'app/components/pages/home/home.component';
import { PageNotFoundComponent } from 'app/components/pages/page-not-found/page-not-found.component';
import { PrivacyComponent } from 'app/components/pages/privacy/privacy.component';
import { EthicsComponent } from 'app/components/pages/ethics/ethics.component';
import { ShowArchiveComponent } from './components/pages/show-archive/show-archive.component';
import { ContactComponent } from 'app/components/pages/contact/contact.component';
import { FaqComponent } from 'app/components/pages/faq/faq.component';
import { CommunityGuidelinesComponent } from 'app/components/pages/community-guidelines/community-guidelines.component';
import { ShowDetailComponent } from 'app/components/pages/show-detail/show-detail.component';

const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'community-guidelines', component: CommunityGuidelinesComponent },
  { path: 'faq', component: FaqComponent },
  { path: 'privacy', component: PrivacyComponent },
  { path: 'show-archive', component: ShowArchiveComponent },
  { path: 'show-archive/:keyword', component: ShowArchiveComponent },
  { path: 'show/:slug', component: ShowDetailComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'ethics', component: EthicsComponent },
  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, { scrollPositionRestoration: 'enabled' })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
