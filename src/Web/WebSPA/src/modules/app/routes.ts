import { RouteRecordRaw } from "vue-router";
import LayoutVue from "./components/Layout.vue";

import AppListVue from "./views/AppList.vue";
import AppCreateVue from "./views/AppCreate.vue";
import AppHomeVue from "./views/AppHome.vue";
import AppDetailsVue from "./views/AppDetails.vue";
import AppLogVue from "./views/AppLog.vue";

import RouteViewVue from "./views/routes/RouteView.vue";
import RouteViewHomeVue from "./views/routes/RouteViewHome.vue";
import RouteEditorV2Vue from "./views/routes/RouteEditorV2.vue";
import RouteAuthorizationVue from "./views/routes/RouteAuthorization.vue";
import RouteResponse from "./views/routes/RouteResponse.vue";

const routes: RouteRecordRaw[] = [
  { path: "/app", name: "AppList", component: AppListVue, meta: { layout: LayoutVue } },
  { path: "/app/new", name: "AppCreate", component: AppCreateVue, meta: { layout: LayoutVue } },
  {
    path: "/app/:id",
    name: "AppHome",
    component: AppHomeVue,
    children: [
      { path: "", name: "Overview", component: AppDetailsVue },
      {
        path: "route",
        name: "RouteView",
        component: RouteViewVue,
        children: [
          { path: "", component: RouteViewHomeVue },
          { path: "new", name: "NewRoute", component: RouteEditorV2Vue },
          { path: ":routeId", name: "EditRoute", component: RouteEditorV2Vue },
          { path: ":routeId/response", name: 'RouteResponse', component: RouteResponse },
          { path: ":routeId/authorization", component: RouteAuthorizationVue },
        ],
      },
      {
        path: "log",
        name: "AppLog",
        component: AppLogVue,
      },
    ],
    meta: { layout: LayoutVue }
  },
];

export default routes;
