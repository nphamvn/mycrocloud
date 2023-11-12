import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router';
import './userWorker';
import { createPinia } from 'pinia'

import layouts from './layouts/plugin';
import { createAuth0 } from '@auth0/auth0-vue';

const pinia = createPinia();
const auth0 = createAuth0({
  domain: 'dev-vzxphouz.us.auth0.com',
  clientId: '8YECKUMM3I7ejQgsVky40b5LooQB0YFf',
  authorizationParams: {
    audience: 'https://mycrocloud.com',
    redirect_uri: 'http://localhost:5173/callback'
  }
})
const app = createApp(App)

app.use(router)
  .use(layouts)
  .use(pinia)
  .use(auth0)
  ;
app.mount('#app')
