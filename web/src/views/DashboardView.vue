<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 to-orange-100 py-8">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header Section -->
      <div class="text-center mb-12">
        <h1 class="text-4xl font-bold text-gray-900 mb-4">
          Your Dashboard
        </h1>
        <p class="text-xl text-gray-600 max-w-2xl mx-auto">
          Your to-do lists will be tracked here. Start adding your first list to get organized and boost your productivity.
        </p>
      </div>

      <!-- Lists Content -->
      <div class="bg-white rounded-2xl shadow-xl p-8">
        <ListManager />
      </div>
    </div>
  </div>
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