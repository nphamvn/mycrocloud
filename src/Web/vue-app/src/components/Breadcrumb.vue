<template>
    <nav class="flex" aria-label="Breadcrumb">
        <ol class="inline-flex items-center space-x-1 md:space-x-3">
            <li v-for="item in items" class="inline-flex items-center">
                <a :href="item.link"
                    class="inline-flex items-center text-sm font-medium text-gray-700 hover:text-blue-600 dark:text-gray-400 dark:hover:text-white">
                    {{ item.title }}
                </a>
            </li>
        </ol>
    </nav>
</template>
<script setup lang="ts">
interface BreadcrumbItem {
    title: string;
    link?: string;
}
const { items } = defineProps<{
    items: BreadcrumbItem[]
}>()
</script>