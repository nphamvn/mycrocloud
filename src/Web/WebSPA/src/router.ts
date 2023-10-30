import Home from './components/Home.vue'
import AppList from './components/AppList.vue'
import AppCreate from './components/AppCreate.vue';
import AppHome from './components/AppHome.vue';
import AppDetails from './components/AppDetails.vue';
import RouteView from './components/RouteView.vue';
import NoteHome from './components/NoteHome.vue'
import RouteEditorV2 from './components/RouteEditorV2.vue'
import RouteViewHome from './components/RouteViewHome.vue'
import { RouteRecordRaw, createRouter, createWebHashHistory } from 'vue-router';
import RouteMockResponse from './components/RouteMockResponse.vue';
import RouteAuthorization from './components/RouteAuthorization.vue';

const routes: RouteRecordRaw[] = [
  { path: '/', component: Home },
  { path: '/app', component: AppList, name: 'AppList' },
  { path: '/app/new', component: AppCreate },
  {
    path: '/app/:id', component: AppHome, children: [
      { path: '', name: 'Overview' ,component: AppDetails },
      {
        path: 'route', name: 'RouteView', component: RouteView, children: [
          { path: '', component: RouteViewHome },
          { path: 'new', name: 'NewRoute', component: RouteEditorV2 },
          { path: ':routeId', name: 'EditRoute', component: RouteEditorV2 },
          { path: ':routeId/response/mock', component: RouteMockResponse },
          { path: ':routeId/authorization', component: RouteAuthorization }
        ]
      }
    ]
  },
  { path: '/note', component: NoteHome },
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})

export default router;