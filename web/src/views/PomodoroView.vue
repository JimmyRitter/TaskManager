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
          <!-- Session Type Selector -->
          <div class="flex justify-center mb-8">
            <div class="bg-gray-100 p-1 rounded-xl flex">
              <button
                @click="setFocus"
                :class="[
                  'px-6 py-3 rounded-lg font-semibold transition-all duration-200',
                  !isBreak 
                    ? 'bg-orange-500 text-white shadow-md' 
                    : 'text-gray-600 hover:text-gray-800'
                ]"
              >
                Work Session
              </button>
              <button
                @click="setBreak"
                :class="[
                  'px-6 py-3 rounded-lg font-semibold transition-all duration-200',
                  isBreak 
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
                {{ isBreak ? 'Break Time' : 'Focus Time' }}
              </span>
            </div>
            <div class="text-7xl font-mono font-bold text-gray-900 mb-6 tracking-wider">
              {{ String(minutes).padStart(2, '0') }}:{{ String(seconds).padStart(2, '0') }}
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
                    :class="isBreak ? 'text-blue-500' : 'text-orange-500'"
                    :stroke-dasharray="circumference"
                    :stroke-dashoffset="strokeDashoffset"
                    class="transition-all duration-1000 ease-out"
                  />
                </svg>
                <div class="absolute inset-0 flex items-center justify-center">
                  <span class="text-2xl font-bold text-gray-700">
                    {{ Math.round(progress * 100) }}%
                  </span>
                </div>
              </div>
            </div>

            <!-- Control Buttons -->
            <div class="flex justify-center gap-4">
              <button
                @click="toggleTimer"
                :class="[
                  'px-8 py-4 rounded-xl font-semibold text-lg transition-all duration-200 shadow-lg',
                  isRunning
                    ? 'bg-red-500 hover:bg-red-600 text-white'
                    : 'bg-orange-500 hover:bg-orange-600 text-white'
                ]"
              >
                {{ isRunning ? '⏸️ Pause' : '▶️ Start' }}
              </button>
              <button
                @click="resetTimer"
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
              <div class="text-3xl font-bold text-orange-600 mb-2">{{ focusSessions }}</div>
              <div class="text-gray-600 font-medium">Completed Sessions</div>
            </div>
            <div class="text-center p-4 bg-blue-50 rounded-xl">
              <div class="text-3xl font-bold text-blue-600 mb-2">{{ focusHours }}h {{ focusMinutes }}m</div>
              <div class="text-gray-600 font-medium">Total Focus Time</div>
            </div>
            <div class="text-center p-4 bg-green-50 rounded-xl">
              <div class="text-3xl font-bold text-green-600 mb-2">{{ Math.round((focusSessions / 8) * 100) }}%</div>
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
                <p>Focus on a single task for {{ FOCUS_DURATION }} minutes without distractions.</p>
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
    <div v-if="showNotification" class="fixed bottom-4 right-4 z-50">
      <div class="bg-orange-500 text-white px-6 py-4 rounded-lg shadow-lg font-medium">
        {{ notificationMessage }}
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted, onMounted } from 'vue'

const FOCUS_DURATION = 25; // 25 minutes for proper Pomodoro

const WORK_DURATION = FOCUS_DURATION * 60
const BREAK_DURATION = 5 * 60 // 5 minutes

const timeLeft = ref(WORK_DURATION)
const totalDuration = ref(WORK_DURATION)
const isBreak = ref(false)
const isRunning = ref(false)
const showNotification = ref(false)
const notificationMessage = ref('')
let interval: number | undefined

const focusSessions = ref(0)
const minutes = computed(() => Math.floor(timeLeft.value / 60))
const seconds = computed(() => timeLeft.value % 60)

const focusMinutesTotal = computed(() => focusSessions.value * FOCUS_DURATION)
const focusHours = computed(() => Math.floor(focusMinutesTotal.value / 60))
const focusMinutes = computed(() => focusMinutesTotal.value % 60)

// Progress calculation for the circular progress bar
const progress = computed(() => {
  return (totalDuration.value - timeLeft.value) / totalDuration.value
})

const circumference = 2 * Math.PI * 54 // radius = 54
const strokeDashoffset = computed(() => {
  return circumference - (progress.value * circumference)
})

let originalTitle = document.title

function updateTitle() {
  const label = isBreak.value ? 'Break' : 'Focus'
  const timeString = `${String(minutes.value).padStart(2, '0')}:${String(seconds.value).padStart(2, '0')}`
  document.title = `${timeString} - ${label} | TaskManager`
}

function showToast(message: string) {
  notificationMessage.value = message
  showNotification.value = true
  setTimeout(() => {
    showNotification.value = false
  }, 3000)
}

watch([minutes, seconds, isBreak], updateTitle)

onMounted(() => {
  originalTitle = document.title
  updateTitle()
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
  document.title = originalTitle
})

function startTimer() {
  if (!isRunning.value) {
    isRunning.value = true
    interval = setInterval(() => {
      if (timeLeft.value > 0) {
        timeLeft.value--
      } else {
        switchSession()
      }
    }, 1000)
  }
}

function pauseTimer() {
  isRunning.value = false
  if (interval) clearInterval(interval)
}

function resetTimer() {
  pauseTimer()
  const duration = isBreak.value ? BREAK_DURATION : WORK_DURATION
  timeLeft.value = duration
  totalDuration.value = duration
}

function toggleTimer() {
  if (isRunning.value) {
    pauseTimer()
  } else {
    startTimer()
  }
}

function switchSession() {
  pauseTimer()
  
  if (!isBreak.value) {
    focusSessions.value++
    showToast('🎉 Work session complete! Time for a break.')
    isBreak.value = true
    timeLeft.value = BREAK_DURATION
    totalDuration.value = BREAK_DURATION
  } else {
    showToast('✨ Break over! Ready for another focus session?')
    isBreak.value = false
    timeLeft.value = WORK_DURATION
    totalDuration.value = WORK_DURATION
  }
  
  // Auto-start the next session
  setTimeout(() => {
    startTimer()
  }, 1000)
}

function setFocus() {
  pauseTimer()
  isBreak.value = false
  timeLeft.value = WORK_DURATION
  totalDuration.value = WORK_DURATION
}

function setBreak() {
  pauseTimer()
  isBreak.value = true
  timeLeft.value = BREAK_DURATION
  totalDuration.value = BREAK_DURATION
}
</script> 