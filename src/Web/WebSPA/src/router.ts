import { RouteRecordRaw, createRouter, createWebHashHistory } from "vue-router";

import Home from "./views/Home.vue";
import AppList from "./views/apps/AppList.vue";
import AppCreate from "./views/apps/AppCreate.vue";
import AppHome from "./views/apps/AppHome.vue";
import AppDetails from "./views/apps/AppDetails.vue";
import AppLog from "./views/apps/AppLog.vue";
import RouteView from "./views/apps/routes/RouteView.vue";
import NoteHome from "./views/notes/NoteHome.vue";
import RouteEditorV2 from "./views/apps/routes/RouteEditorV2.vue";
import RouteViewHome from "./views/apps/routes/RouteViewHome.vue";
import RouteMockResponse from "./views/apps/routes/RouteMockResponse.vue";
import RouteAuthorization from "./views/apps/routes/RouteAuthorization.vue";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "Home", component: Home },
  { path: "/app", name: "AppList", component: AppList },
  { path: "/app/new", name: "AppCreate", component: AppCreate },
  {
    path: "/app/:id",
    name: "AppHome",
    component: AppHome,
    children: [
      { path: "", name: "Overview", component: AppDetails },
      {
        path: "route",
        name: "RouteView",
        component: RouteView,
        children: [
          { path: "", component: RouteViewHome },
          { path: "new", name: "NewRoute", component: RouteEditorV2 },
          { path: ":routeId", name: "EditRoute", component: RouteEditorV2 },
          { path: ":routeId/response/mock", component: RouteMockResponse },
          { path: ":routeId/authorization", component: RouteAuthorization },
        ],
      },
      {
        path: "log",
        name: "AppLog",
        component: AppLog
      }
    ],
  },
  { path: "/note", component: NoteHome },
];

const router = createRouter({
  history: createWebHashHistory(),
  routes,
});

export default router;
