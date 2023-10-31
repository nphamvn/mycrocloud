<template>
  <div class="h-full">
    <header class="bg-violet-600 text-white px-3 py-2">
      <nav>
        <ul class="flex flex-row space-x-4">
          <li><router-link to="/">MycroCloud</router-link></li>
          <li><router-link to="app">App</router-link></li>
          <li><router-link to="/note">Note</router-link></li>
          <li v-if="isLoading">isLoading</li>
          <li v-if="!isAuthenticated">
            <button v-if="!isAuthenticated" @click="login">Log in</button>
          </li>
          <li v-else>
            <button id="dropdownNavbarLink" data-dropdown-toggle="dropdownNavbar"
              class="flex items-center justify-between w-full py-2 pl-3 pr-4 rounded hover:text-slate-100 md:border-0 md:p-0 md:w-auto dark:text-white md:dark:hover:text-blue-500 dark:focus:text-white dark:border-gray-700 dark:hover:bg-gray-700 md:dark:hover:bg-transparent">
              {{ user?.name }}
              <svg class="w-2.5 h-2.5 ml-2.5" aria-hidden="true" xmlns="http://www.w3.org/2000/svg" fill="none"
                viewBox="0 0 10 6">
                <path stroke="currentColor" stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                  d="m1 1 4 4 4-4" />
              </svg></button>
            <!-- Dropdown menu -->
            <div id="dropdownNavbar"
              class="z-10 hidden font-normal bg-white divide-y divide-gray-100 rounded-lg shadow w-44 dark:bg-gray-700 dark:divide-gray-600">
              <ul class="py-2 text-sm text-gray-700 dark:text-gray-400" aria-labelledby="dropdownLargeButton">
                <li>
                  <a href="#"
                    class="block px-4 py-2 hover:bg-gray-100 dark:hover:bg-gray-600 dark:hover:text-white">Settings</a>
                </li>
              </ul>
              <div class="py-1">
                <a href="#"
                  class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 dark:hover:bg-gray-600 dark:text-gray-400 dark:hover:text-white">Sign
                  out</a>
              </div>
            </div>
          </li>
        </ul>
      </nav>
    </header>
    <div class="container px-4 py-5 h-full">
      <router-view></router-view>
    </div>
    <div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { nextTick, onMounted, watch } from 'vue';
import { initFlowbite } from 'flowbite'

import { useAuth } from './auth';

const { 
  loginWithRedirect, 
  logout, user, isAuthenticated, isLoading, getAccessTokenSilently } 
= useAuth0();
const login = () => {
  loginWithRedirect();
}
// const logout = () => {
//   logout({ logoutParams: { returnTo: window.location.origin } });
// }
watch(isLoading, async () => {
  console.log(isLoading.value)
  if (!isLoading) {
    //await nextTick();
    //initFlowbite()
  }
})
onMounted(async () => {
  const token = await getAccessTokenSilently();
  console.log(token);
})
</script>