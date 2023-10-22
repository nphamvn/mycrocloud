<template>
    <v-row class="ma-0">
        <v-col cols="3" class="border">
            <div>
                <v-btn @click="openRoute(undefined)">New</v-btn>
            </div>
            <div>
                <v-text-field placeholder="Search">

                </v-text-field>
            </div>
            <v-divider></v-divider>
            <div>
                <v-list>
                    <v-list-item v-for="route in routes" :title="route.name"
                        :class="route.id === openingRoute?.id ? 'v-list-item-active' : ''"
                        v-on:click="openRoute(route.id)">
                    </v-list-item>
                </v-list>
            </div>
        </v-col>
        <v-col class="border">
            <RouteEditor v-if="openingRoute" />
        </v-col>
    </v-row>
</template>
<script setup lang="ts" >
import { onMounted } from 'vue';
import { ref } from 'vue';
import RouteEditor from './RouteEditor.vue';

interface RouteItem {
    id?: number;
    name?: string;
    template?: string;
    method?: string;
}

const routes = ref<RouteItem[]>([]);
const openingRoute = ref<RouteItem>();

onMounted(() => {
    routes.value = Array.from({ length: 10 }, (_, index) => ({
        id: index + 1,
        name: `Route ${index + 1}`,
        template: '',
        method: 'GET'
    }))
})

const openRoute = (id: number | undefined) => {
    if (id) {
        openingRoute.value = routes.value.find(r => r.id === id);
    }
    else {
        openingRoute.value = {
            name: 'Foo',
            template: '/foo',
            method: 'ANY'
        };
    }
}

</script>
<style scoped>
.v-list-item-active {
    background-color: aliceblue;
}
</style>