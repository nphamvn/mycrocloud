import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router';
import './userWorker';
import { createPinia } from 'pinia'
import { createAuth0 } from '@auth0/auth0-vue';
const pinia = createPinia();
const auth0 = createAuth0({
  domain: 'dev-vzxphouz.us.auth0.com',
  clientId: 'v12ox086oc0ZfbTaLIAbiohJZWgwzwqa',
  authorizationParams: {
    redirect_uri: window.location.origin
  }
})

const app = createApp(App);
app.use(router)
  .use(pinia)
  .use(auth0);

app.mount('#app')
