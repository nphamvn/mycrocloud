<template>
  <div>
    <header class="bg-violet-600 text-white px-3 py-2">
      <nav>
        <ul>
          <li><router-link to="/">MycroCloud</router-link></li>
        </ul>
      </nav>

    </header>
    <div class="container px-4 py-5">
      <router-view></router-view>
    </div>
    <div>

    </div>
  </div>
</template>
