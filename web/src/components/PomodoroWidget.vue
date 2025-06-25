<template>
  <Teleport to="body">
    <Transition
      enter-active-class="transition-all duration-300 ease-out"
      enter-from-class="transform translate-y-full opacity-0"
      enter-to-class="transform translate-y-0 opacity-100"
      leave-active-class="transition-all duration-300 ease-in"
      leave-from-class="transform translate-y-0 opacity-100"
      leave-to-class="transform translate-y-full opacity-0"
    >
      <div
        v-if="shouldShowWidget"
        class="fixed bottom-4 right-4 z-50"
      >
        <div
          class="bg-white rounded-2xl shadow-xl border border-gray-200 p-4 min-w-[280px] cursor-pointer hover:shadow-2xl transition-shadow duration-200"
          @click="navigateToPomodoro"
        >
          <!-- Header -->
          <div class="flex items-center justify-between mb-3">
            <div class="flex items-center space-x-2">
              <div
                :class="[
                  'w-3 h-3 rounded-full',
                  pomodoroStore.isBreak ? 'bg-blue-500' : 'bg-orange-500'
                ]"
              ></div>
              <span class="font-semibold text-gray-900">
                {{ pomodoroStore.isBreak ? 'Break Time' : 'Focus Time' }}
              </span>
            </div>
            <button
              @click.stop="toggleTimer"
              :class="[
                'p-1.5 rounded-lg transition-colors duration-200',
                pomodoroStore.isRunning 
                  ? 'text-red-500 hover:bg-red-50' 
                  : 'text-green-500 hover:bg-green-50'
              ]"
            >
              <svg v-if="pomodoroStore.isRunning" class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M6 4a1 1 0 011 1v10a1 1 0 01-2 0V5a1 1 0 011-1zM14 4a1 1 0 011 1v10a1 1 0 01-2 0V5a1 1 0 011-1z" clip-rule="evenodd" />
              </svg>
              <svg v-else class="w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M10 2a8 8 0 100 16 8 8 0 000-16zM9.555 7.168A1 1 0 008 8v4a1 1 0 001.555.832l3-2a1 1 0 000-1.664l-3-2z" clip-rule="evenodd" />
              </svg>
            </button>
          </div>

          <!-- Timer Display -->
          <div class="text-center mb-3">
            <div class="text-2xl font-mono font-bold text-gray-900">
              {{ String(pomodoroStore.minutes).padStart(2, '0') }}:{{ String(pomodoroStore.seconds).padStart(2, '0') }}
            </div>
          </div>

          <!-- Progress Bar -->
          <div class="w-full bg-gray-200 rounded-full h-2 mb-3">
            <div
              :class="[
                'h-2 rounded-full transition-all duration-1000 ease-out',
                pomodoroStore.isBreak ? 'bg-blue-500' : 'bg-orange-500'
              ]"
              :style="{ width: `${pomodoroStore.progress * 100}%` }"
            ></div>
          </div>

          <!-- Stats -->
          <div class="flex justify-between text-xs text-gray-500">
            <span>{{ pomodoroStore.focusSessions }} sessions</span>
            <span>{{ pomodoroStore.focusHours }}h {{ pomodoroStore.focusMinutes }}m total</span>
          </div>
        </div>
      </div>
    </Transition>
  </Teleport>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { usePomodoroStore } from '@/stores/pomodoro'
import { useRouter, useRoute } from 'vue-router'
import { useAuthStore } from '@/stores/auth'

const pomodoroStore = usePomodoroStore()
const router = useRouter()
const route = useRoute()
const authStore = useAuthStore()

const WORK_DURATION = pomodoroStore.FOCUS_DURATION * 60
const BREAK_DURATION = 5 * 60

// Widget should only show if:
// 1. User is authenticated
// 2. Timer is running or has been started (time is less than full duration)
// 3. User is NOT currently on the Pomodoro page
const shouldShowWidget = computed(() => {
  const isAuthenticated = !!authStore.user
  const timerHasBeenStarted = pomodoroStore.isRunning || 
    pomodoroStore.timeLeft < (pomodoroStore.isBreak ? BREAK_DURATION : WORK_DURATION)
  const isNotOnPomodoroPage = route.name !== 'pomodoro'
  
  return isAuthenticated && timerHasBeenStarted && isNotOnPomodoroPage
})

function toggleTimer() {
  pomodoroStore.toggleTimer()
}

function navigateToPomodoro() {
  router.push('/pomodoro')
}
</script> 