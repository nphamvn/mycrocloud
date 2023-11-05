<template>
    <div class="p-3 border">
        <div class="border rounded-md">
            <h5 class="bg-gray-100 p-2">{{ app?.name }}</h5>
            <div class="p-2">
                <p class="font-normal text-gray-700 dark:text-gray-400">{{ app?.description }} font-normal text-gray-700 dark:text-gray-400</p>
                <p>Created at: {{ app?.createdAt.toDateString() }}</p>
                <p>Route Count: {{ app?.routeCount }}</p>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { useAppStore } from '../store/appStore';
import { storeToRefs } from 'pinia';
import { FwbCard } from 'flowbite-vue'

const { currentApp: app } = storeToRefs(useAppStore());
</script>