<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 to-orange-100 py-8">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header Section -->
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-900 mb-4">
          Pomodoro Timer
        </h1>
        <p class="text-xl text-gray-600">
          Stay focused with the proven Pomodoro Technique
        </p>
      </div>

      <!-- Main Timer Card -->
      <div class="max-w-2xl mx-auto">
        <div class="bg-white rounded-2xl shadow-xl p-8 mb-8">
          <!-- Testing Controls (Development Only) -->
          <div v-if="isDevelopment" class="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-6">
            <h4 class="text-sm font-medium text-yellow-800 mb-3">🧪 Testing Mode</h4>
            <div class="flex flex-wrap gap-2 justify-center">
              <button
                @click="setTestDurations(10/60, 5/60)"
                :class="[
                  'px-3 py-1 rounded text-xs transition-colors',
                  isSelectedDuration(10/60, 5/60) 
                    ? 'bg-yellow-200 text-yellow-900 border-2 border-yellow-400' 
                    : 'bg-yellow-100 text-yellow-800 hover:bg-yellow-200'
                ]"
              >
                10s / 5s
              </button>
              <button
                @click="setTestDurations(25, 5)"
                :class="[
                  'px-3 py-1 rounded text-xs transition-colors',
                  isSelectedDuration(25, 5) 
                    ? 'bg-yellow-200 text-yellow-900 border-2 border-yellow-400' 
                    : 'bg-yellow-100 text-yellow-800 hover:bg-yellow-200'
                ]"
              >
                25min / 5min (default)
              </button>
            </div>
          </div>



          <!-- Session Type Selector -->
          <div class="flex justify-center mb-8">
            <div class="bg-gray-100 p-1 rounded-xl flex">
              <button
                @click="pomodoroStore.setFocus"
                :class="[
                  'px-6 py-3 rounded-lg font-semibold transition-all duration-200',
                  !pomodoroStore.isBreak 
                    ? 'bg-orange-500 text-white shadow-md' 
                    : 'text-gray-600 hover:text-gray-800'
                ]"
              >
                Work Session
              </button>
              <button
                @click="pomodoroStore.setBreak"
                :class="[
                  'px-6 py-3 rounded-lg font-semibold transition-all duration-200',
                  pomodoroStore.isBreak 
                    ? 'bg-orange-500 text-white shadow-md' 
                    : 'text-gray-600 hover:text-gray-800'
                ]"
              >
                Short Break
              </button>
            </div>
          </div>

          <!-- Timer Display -->
          <div class="text-center mb-8">
            <div class="mb-4">
              <span class="inline-block px-4 py-2 bg-orange-100 text-orange-800 rounded-full text-lg font-semibold">
                {{ pomodoroStore.isBreak ? 'Break Time' : 'Focus Time' }}
              </span>
            </div>
            <div class="text-7xl font-mono font-bold text-gray-900 mb-6 tracking-wider">
              {{ String(pomodoroStore.minutes).padStart(2, '0') }}:{{ String(pomodoroStore.seconds).padStart(2, '0') }}
            </div>
            
            <!-- Progress Ring -->
            <div class="flex justify-center mb-8">
              <div class="relative w-32 h-32">
                <svg class="w-32 h-32 transform -rotate-90" viewBox="0 0 120 120">
                  <!-- Background circle -->
                  <circle
                    cx="60"
                    cy="60"
                    r="54"
                    stroke="currentColor"
                    stroke-width="8"
                    fill="none"
                    class="text-gray-200"
                  />
                  <!-- Progress circle -->
                  <circle
                    cx="60"
                    cy="60"
                    r="54"
                    stroke="currentColor"
                    stroke-width="8"
                    fill="none"
                    stroke-linecap="round"
                    :class="pomodoroStore.isBreak ? 'text-blue-500' : 'text-orange-500'"
                    :stroke-dasharray="circumference"
                    :stroke-dashoffset="strokeDashoffset"
                    class="transition-all duration-1000 ease-out"
                  />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <span class="text-2xl font-bold text-gray-700">
                    {{ Math.round(pomodoroStore.progress * 100) }}%
                  </span>
                </div>
              </div>
            </div>

            <!-- Control Buttons -->
            <div class="flex justify-center gap-4">
              <button
                @click="pomodoroStore.toggleTimer"
                :class="[
                  'px-8 py-4 rounded-xl font-semibold text-lg transition-all duration-200 shadow-lg',
                  pomodoroStore.isRunning
                    ? 'bg-red-500 hover:bg-red-600 text-white'
                    : 'bg-orange-500 hover:bg-orange-600 text-white'
                ]"
              >
                {{ pomodoroStore.isRunning ? '⏸️ Pause' : '▶️ Start' }}
              </button>
              <button
                @click="pomodoroStore.resetTimer"
                class="px-8 py-4 bg-gray-200 hover:bg-gray-300 text-gray-700 rounded-xl font-semibold text-lg transition-all duration-200 shadow-lg"
              >
                🔄 Reset
              </button>
            </div>
          </div>
        </div>

        <!-- Statistics Card -->
        <div class="bg-white rounded-2xl shadow-xl p-8 mb-8">
          <h3 class="text-2xl font-semibold text-gray-900 mb-6 text-center">Today's Progress</h3>
          <div class="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div class="text-center p-4 bg-orange-50 rounded-xl">
              <div class="text-3xl font-bold text-orange-600 mb-2">{{ pomodoroStore.focusSessions }}</div>
              <div class="text-gray-600 font-medium">Completed Sessions</div>
            </div>
            <div class="text-center p-4 bg-blue-50 rounded-xl">
              <div class="text-3xl font-bold text-blue-600 mb-2">{{ pomodoroStore.focusHours }}h {{ pomodoroStore.focusMinutes }}m</div>
              <div class="text-gray-600 font-medium">Total Focus Time</div>
            </div>
            <div class="text-center p-4 bg-green-50 rounded-xl">
              <div class="text-3xl font-bold text-green-600 mb-2">{{ Math.round((pomodoroStore.focusSessions / 8) * 100) }}%</div>
              <div class="text-gray-600 font-medium">Daily Goal (8 sessions)</div>
            </div>
          </div>
        </div>

        <!-- Info Card -->
        <div class="bg-white rounded-2xl shadow-xl p-8">
          <h3 class="text-2xl font-semibold text-gray-900 mb-4 text-center">How it Works</h3>
          <div class="grid grid-cols-1 md:grid-cols-2 gap-6 text-gray-600">
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-orange-100 rounded-full flex items-center justify-center">
                <span class="text-orange-600 font-bold">1</span>
              </div>
              <div>
                <h4 class="font-semibold text-gray-900 mb-1">Work Session</h4>
                <p>Focus on a single task for {{ pomodoroStore.FOCUS_DURATION_MINUTES }} minutes without distractions.</p>
              </div>
            </div>
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-blue-100 rounded-full flex items-center justify-center">
                <span class="text-blue-600 font-bold">2</span>
              </div>
              <div>
                <h4 class="font-semibold text-gray-900 mb-1">Short Break</h4>
                <p>Take a 5-minute break to rest and recharge your mind.</p>
              </div>
            </div>
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-green-100 rounded-full flex items-center justify-center">
                <span class="text-green-600 font-bold">3</span>
              </div>
              <div>
                <h4 class="font-semibold text-gray-900 mb-1">Repeat</h4>
                <p>Continue the cycle. After 4 sessions, take a longer 15-30 minute break.</p>
              </div>
            </div>
            <div class="flex items-start space-x-3">
              <div class="flex-shrink-0 w-8 h-8 bg-purple-100 rounded-full flex items-center justify-center">
                <span class="text-purple-600 font-bold">4</span>
              </div>
              <div>
                <h4 class="font-semibold text-gray-900 mb-1">Stay Consistent</h4>
                <p>The key to success is maintaining regular, focused work sessions.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Notification Toast -->
    <div v-if="pomodoroStore.showNotification" class="fixed bottom-4 right-4 z-50">
      <div class="bg-orange-500 text-white px-6 py-4 rounded-lg shadow-lg font-medium">
        {{ pomodoroStore.notificationMessage }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { usePomodoroStore } from '@/stores/pomodoro'

const pomodoroStore = usePomodoroStore()

// Check if we're in development mode
const isDevelopment = computed(() => import.meta.env.DEV)

// Progress calculation for the circular progress bar
const circumference = 2 * Math.PI * 54 // radius = 54
const strokeDashoffset = computed(() => {
  return circumference - (pomodoroStore.progress * circumference)
})

onMounted(() => {
  pomodoroStore.initializeTitle()
  
  // Auto-request notification permission
  if ('Notification' in window && Notification.permission === 'default') {
    pomodoroStore.requestNotificationPermission()
  }
})

// Helper function to set test durations
function setTestDurations(focusMinutes: number, breakMinutes: number) {
  pomodoroStore.setFocusDuration(focusMinutes)
  pomodoroStore.setBreakDuration(breakMinutes)
}

// Check if current durations match the given preset
function isSelectedDuration(focusMinutes: number, breakMinutes: number) {
  return Math.abs(pomodoroStore.FOCUS_DURATION_MINUTES - focusMinutes) < 0.01 &&
         Math.abs(pomodoroStore.BREAK_DURATION_MINUTES - breakMinutes) < 0.01
}


</script> 