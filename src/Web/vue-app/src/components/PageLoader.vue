<template>
    <div class="loader">
        <img :src="loadingImg" alt="Loading..." />
    </div>
</template>

<script setup lang="ts">
const loadingImg = "https://cdn.auth0.com/blog/hello-auth0/loader.svg";
</script>