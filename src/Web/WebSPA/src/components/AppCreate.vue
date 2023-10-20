<template>
    <v-container>
        <h1>Create new app</h1>
        <v-form @submit.prevent="submit" class="mt-3">
            <v-text-field v-model="inputs.name" label="Name" :error-messages="name.errorMessage.value"
                aria-autocomplete="none">
            </v-text-field>
            <v-textarea v-model="inputs.description" label="Description" :error-messages="description.errorMessage.value">
            </v-textarea>
            <v-btn type="submit">Create
            </v-btn>
        </v-form>
    </v-container>
</template>
<script setup lang="ts">
import { ref } from 'vue';
import { useForm, useField } from 'vee-validate';
import * as yup from 'yup';

interface Inputs {
    name: string;
    description: string;
}

const inputs = ref<Inputs>({
    name: '',
    description: ''
});

const validationSchema = yup.object({
    name: yup.string().required('App name is required'),
    description: yup.string().max(10, 'Description must be in 10 characters')
})
const { defineInputBinds, handleSubmit, errors } = useForm({
    validationSchema: validationSchema,
});
const name = useField('name')
const description = useField('description')

const submit = handleSubmit(values => {
    alert(JSON.stringify(values, null, 2))
})
</script>