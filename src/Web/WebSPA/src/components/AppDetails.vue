<template>
    <h1>App {{ appId }}</h1>
</template>
<script setup lang="ts">
import { useRoute } from 'vue-router';

const appId = useRoute().params['id'];

</script>