import { createApp } from 'vue'
import { RouteRecordRaw, createRouter, createWebHashHistory } from 'vue-router'
import './style.css'
import App from './App.vue'
import Home from './components/Home.vue'
import AppList from './components/AppList.vue'
import AppCreate from './components/AppCreate.vue';
import AppHome from './components/AppHome.vue';
import AppDetails from './components/AppDetails.vue';
import RouteView from './components/RouteView.vue';

// Vuetify
import 'vuetify/styles'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import { aliases, mdi } from 'vuetify/iconsets/mdi'
import '@mdi/font/css/materialdesignicons.css'

const routes: RouteRecordRaw[] = [
  { path: '/', component: Home },
  { path: '/apps', component: AppList, name: 'AppList' },
  { path: '/apps/new', component: AppCreate },
  {
    path: '/apps/:id', component: AppHome, children: [
      { path: '', component: AppDetails },
      { path: 'routes', component: RouteView }
    ]
  }
]

const router = createRouter({
  history: createWebHashHistory(),
  routes,
})



const vuetify = createVuetify({
  components,
  directives,
  icons: {
    defaultSet: 'mdi',
    aliases: aliases,
    sets : {
      mdi
    }
  }
})

const app = createApp(App);
app.use(router).use(vuetify);

app.mount('#app')
