<template>
    <div>
        <v-breadcrumbs :items="[{ title: 'Apps', exact: true, to: { name: 'AppList' } }, { title: app?.name! }]"
            divider="/"></v-breadcrumbs>
        <div class="w-100 border-b">
            <router-link :to="`/apps/${appId}`"><v-btn class="px-6" rounded="0">Home</v-btn></router-link>
            <router-link :to="`/apps/${appId}/routes`"><v-btn class="px-6" rounded="0">Routes</v-btn></router-link>
        </div>
        <div class="mt-4">
            <router-view></router-view>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import AppItem from '../AppItem';
import { onMounted } from 'vue';

const appId = parseInt(useRoute().params['id'].toString());
const app = ref<AppItem>();

onMounted(() => {
    app.value = {
        id: appId,
        name: `App ${appId}`,
        createdAt: new Date()
    }
    document.title = app.value.name;
});

</script>