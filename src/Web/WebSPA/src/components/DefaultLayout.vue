<template>
    <div class="h-full">
        <nav class="">
            <div class="flex flex-wrap items-center justify-between px-4 py-2">
                <a href="#" class="flex items-center">
                    <span class="self-center text-xl font-semibold whitespace-nowrap">MycroCloud</span>
                </a>
                <ul class="flex font-medium flex-row space-x-8 mt-0">
                    <template v-if="isLoading">
                        <li class="inline-flex items-center">
                            <FwbSpinner />
                        </li>
                    </template>
                    <template v-else>
                        <li v-if="!isAuthenticated">
                            <RouterLink :to="{ name: 'Login' }">
                                <FwbButton color="dark">Login</FwbButton>
                            </RouterLink>
                        </li>
                        <li v-else>
                            <RouterLink :to="{ name: 'AppList' }">
                                <FwbButton>Dashboard</FwbButton>
                            </RouterLink>
                        </li>
                    </template>
                </ul>
            </div>
        </nav>
        <slot />
    </div>
</template>
<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { FwbButton } from 'flowbite-vue';
import { FwbSpinner } from 'flowbite-vue';

import { RouterLink } from 'vue-router';

const { isLoading, isAuthenticated } = useAuth0();
</script>