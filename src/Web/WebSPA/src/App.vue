<template>
  <div v-if="isLoading" class="page-layout">
    <PageLoader />
  </div>
  <AppLayout v-else />
</template>
<script setup lang="ts">
import { useAuth0 } from '@auth0/auth0-vue';
import { watch } from 'vue';


const { isLoading, getAccessTokenSilently, isAuthenticated } = useAuth0();

watch(isLoading, async (value) => {
  if (!value && isAuthenticated) {
    const accessToken = await getAccessTokenSilently();
    await whoami(accessToken);
  }
})

const whoami = async (accessToken: string) => {
  const response = await fetch("http://localhost:5209/whoami", {
    method: "GET",
    headers: {
      Authorization: `Bearer ${accessToken}`
    }
  });

  const data = await response.text();
  console.log(data);
}
</script>