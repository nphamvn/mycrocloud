<template>
    <Header />
    <div class="h-full" >
        <slot />
    </div>
</template>
<script setup lang="ts">
import Header from './Header.vue';
</script>