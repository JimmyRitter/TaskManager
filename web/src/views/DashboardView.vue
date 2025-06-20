<template>
  <section class="dashboard">
    <h2>Todo Lists</h2>
    <ListManager />
  </section>
</template>

<script setup lang="ts">
import ListManager from '@/components/ListManager.vue'
</script>

<style scoped>
.dashboard {
  max-width: 800px;
  margin: 3rem auto;
  padding: 2rem;
}
</style> 