import { RouteRecordRaw, createRouter, createWebHistory } from "vue-router";

import Home from "./views/Home.vue";
import appModuleRoutes from "./modules/app/routes";
import authModuleRoutes from "./modules/auth/routes";
import DefaultLayout from "./components/DefaultLayout.vue";

const routes: RouteRecordRaw[] = [
  { path: "/", name: "Home", component: Home, meta: { layout: DefaultLayout } },
  ...authModuleRoutes,
  ...appModuleRoutes,
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});


router.beforeEach((to, from, next) => {
  console.log(to.name)
  if (to.name !== "Login") {
    //if (to.name !== 'Login' && !isAuthenticated) next({ name: 'Login' })
    next();
  } else {
    next();
  }
});
export default router;
