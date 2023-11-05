import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router';
import './userWorker';
import { createPinia } from 'pinia'

import layouts from './layouts/plugin';
import Spinner from './components/Spinner.vue';

const pinia = createPinia();
const app = createApp(App)
app.component('Spinner', Spinner)

app.use(router)
  .use(layouts)
  .use(pinia)
  ;
app.mount('#app')
