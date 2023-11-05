<template>
    <component :is="layout">
        <router-view></router-view>
    </component>
</template>
<script setup lang="ts">
import { useRoute } from 'vue-router';
import { computed } from 'vue';

const route = useRoute();
const layout = computed(() => {
    const layout = route?.meta?.layout;
    if (layout) {
        return layout;
    }
    return 'div';
});
</script>