import { RouteRecordRaw } from "vue-router";
import Layout from "../../components/DefaultLayout.vue";

import Login from "./views/Login.vue";
import GitHubCallback from "./views/GitHubCallback.vue";
import Auth0Callback from "./views/Auth0Callback.vue";

const routes: RouteRecordRaw[] = [
  {
    path: "/login",
    name: "Login",
    component: Login,
    meta: {
      layout: Layout,
    },
  },
  {
    path: "/signin-github",
    name: "GitHubCallback",
    component: GitHubCallback,
  },
  {
    path: "/signin-auth0",
    name: "Auth0Callback",
    component: Auth0Callback,
  },
];

export default routes;
