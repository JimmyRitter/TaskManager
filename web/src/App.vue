<script setup lang="ts">
import { RouterView } from 'vue-router'
import Navbar from './components/Navbar.vue'
</script>

<template>
  <Navbar />
  <main>
    <RouterView />
  </main>
</template>

<style scoped>
main {
  min-height: calc(100vh - 4rem);
  padding: 2rem 1rem;
}
</style>
