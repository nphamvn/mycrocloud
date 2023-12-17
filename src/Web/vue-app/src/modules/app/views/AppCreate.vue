<template>
    <div>
        <h1>Create new app</h1>
        <form @submit="onSubmit" class="mt-3">
            <div>
                <label class="block">Name</label>
                <input type="text" v-model="name.value.value" aria-autocomplete="none"
                    class="block border border-slate-200 rounded-md" />
                <span class="text-red-500">{{ name.errorMessage.value }}</span>
            </div>
            <div>
                <label class="block">Description</label>
                <textarea v-model="description.value.value"></textarea>
                <span class="text-red-500">{{ description.errorMessage.value }}</span>
            </div>
            <button type="submit">Create</button>
        </form>
    </div>
</template>
<script setup lang="ts">
import { useForm, useField } from 'vee-validate';
import { useRouter } from 'vue-router';
import * as yup from 'yup';

interface Inputs {
    name: string;
    description: string;
}

const router = useRouter();

const validationSchema = yup.object({
    name: yup.string().required('App name is required'),
    description: yup.string().max(400, 'Description must be in 400 characters')
})

const { handleSubmit } = useForm<Inputs>({
    validationSchema: validationSchema,
});

const name = useField<string>('name')
const description = useField<string>('description')

const onSubmit = handleSubmit(values => {
    router.push({ name: 'AppList' });
})
</script>