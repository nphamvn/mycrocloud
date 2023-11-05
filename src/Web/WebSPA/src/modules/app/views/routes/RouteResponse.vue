<template>
    <component :is="component" />
</template>
<script setup lang="ts">
import RouteMockResponse from './RouteMockResponse.vue';
import { useRoute } from 'vue-router';

const type = useRoute().query['type']?.valueOf();
const component = type === 'mock' ? RouteMockResponse : undefined;
</script>