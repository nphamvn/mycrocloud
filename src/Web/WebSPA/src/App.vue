<template>
  <div class="h-full">
    <header class="bg-violet-600 text-white px-3 py-2">
      <nav>
        <ul class="flex space-x-4">
          <li><router-link to="/">MycroCloud</router-link></li>
          <li><router-link to="app">App</router-link></li>
          <li><router-link to="/note">Note</router-link></li>
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
