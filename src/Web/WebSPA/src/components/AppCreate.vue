<template>
    <h1>Create new app</h1>
    <v-form>
        <v-text-field label="Name" v-model="inputs.name">
        </v-text-field>
        <v-btn type="submit">Create
        </v-btn>
    </v-form>
</template>
<script setup lang="ts">
import { ref } from 'vue';
interface Inputs {
    name: string;
}

const inputs = ref<Inputs>({
    name: ''
});

</script>