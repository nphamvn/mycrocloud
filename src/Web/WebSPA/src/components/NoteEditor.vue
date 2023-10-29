<template>
    <div contenteditable="true" class="w-full min-h-screen border border-slate-50 focus:outline-none" aria-autocomplete="none"
        @input="handleInput"
    >
    <div class="flex">
        <input type="checkbox" class="my-auto me-1 border-none" />
        <div contenteditable="true" class="align-middle w-full">Todo Item</div>
    </div>
    <div v-if="showHint">
        <input type="text" @input="handleHintInput" placeholder="Press '/' for commands..." class="w-full border-none">
    </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';

export interface Block {
    type: string;
}
const showHint = ref(false);
const handleInput = () => {
    showHint.value = true;
}
const handleHintInput = (e) => {
    if (e.target.value.startsWith('/')) {
        alert('TODO: Show command')
    } else {
        alert('TODO: Create a Text Block')
        showHint.value = false;
    }
}
</script>