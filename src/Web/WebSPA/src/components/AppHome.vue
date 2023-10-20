<template>
    <v-container>
        <h1>App {{ appId }}</h1>
        <div class="w-100 bg-secondary">
            <router-link :to="`/apps/${appId}`"><v-btn class="px-6" rounded="0">Home</v-btn></router-link>
            <router-link :to="`/apps/${appId}/routes`"><v-btn class="px-6" rounded="0">Routes</v-btn></router-link>
        </div>
        <router-view></router-view>
    </v-container>
</template>
<script setup lang="ts">
import { useRoute } from 'vue-router';

const appId = useRoute().params['id'];
document.title = `App ${appId}`;

</script>