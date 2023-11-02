<template>
    <div ref="modalRef" tabindex="-1"
        class="fixed top-0 left-0 right-0 z-50 hidden p-4 overflow-x-hidden overflow-y-auto md:inset-0 h-[calc(100%-1rem)] max-h-full">
        <div class="relative w-full max-w-md max-h-full">
            <div class="relative bg-white rounded-lg shadow">
                <div class="p-6 text-center">
                    <h3 class="mb-5 text-lg font-normal text-gray-500">Are you sure you
                        want to delete this route?</h3>
                    <button type="button" @click="handleDeleteButtonClick"
                        class="text-white bg-red-600 hover:bg-red-800 focus:ring-4 focus:outline-none focus:ring-red-300 font-medium rounded-lg text-sm inline-flex items-center px-5 py-2.5 text-center mr-2">
                        Delete
                    </button>
                    <button type="button" @click="handleCancleButtonClick"
                        class="text-gray-500 bg-white hover:bg-gray-100 focus:ring-4 focus:outline-none focus:ring-gray-200 rounded-lg border border-gray-200 text-sm font-medium px-5 py-2.5 hover:text-gray-900 focus:z-10">Cancel</button>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { Modal, ModalInterface, ModalOptions } from 'flowbite';
import { onBeforeUnmount, onMounted, ref, defineEmits } from 'vue';
const modalRef = ref();
const emit = defineEmits(['onDeleteClick', 'onCancleClick', 'onHide'])

const options: ModalOptions = {
    placement: 'top-center',
    backdrop: 'dynamic',
    backdropClasses: 'bg-gray-900 bg-opacity-50 dark:bg-opacity-80 fixed inset-0 z-40',
    closable: true,
    onHide: () => {
        console.log('modal is hidden');
        emit('onHide')
    },
    onShow: () => {
        console.log('modal is shown');
    },
    onToggle: () => {
        console.log('modal has been toggled');
    }
};

let modal: ModalInterface;
onMounted(() => {
    console.log('RouteDeleteConfirmModal mounted');
    modal  = new Modal(modalRef.value, options);
    modal.show();
})

onBeforeUnmount(() => {
    console.log('RouteDeleteConfirmModal unmounting...');
    modal.hide();
    modal.destroyAndRemoveInstance();
})

const handleDeleteButtonClick = () => {
    console.log('handleDeleteButtonClick');
    emit('onDeleteClick')
}

const handleCancleButtonClick = () => {
    console.log('handleCancleButtonClick');
    emit('onCancleClick')
}

</script>