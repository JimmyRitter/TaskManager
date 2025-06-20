<template>
  <section class="pomodoro-container">
    <h1>Pomodoro Timer</h1>
    <div class="session-switch">
      <button :class="{ active: !isBreak }" @click="setFocus">Focus</button>
      <button :class="{ active: isBreak }" @click="setBreak">Short Break</button>
    </div>
    <div class="timer-card">
      <div class="timer-label">{{ isBreak ? 'Break' : 'Work Session' }}</div>
      <div class="timer-display">{{ minutes }}:{{ seconds < 10 ? '0' + seconds : seconds }}</div>
      <div class="timer-controls">
        <button @click="toggleTimer">{{ isRunning ? 'Pause' : 'Start' }}</button>
        <button @click="resetTimer">Reset</button>
      </div>
    </div>
    <div class="pomodoro-info">
      <p>Work for {{ FOCUS_DURATION }} minutes, then take a 5 minute break. Repeat to boost your productivity!</p>
      <p v-if="focusSessions > 0" class="focus-summary">
        You focused for {{ focusHours }} {{ focusHours === 1 ? 'hour' : 'hours' }}
        <span v-if="focusMinutes > 0">and {{ focusMinutes }} {{ focusMinutes === 1 ? 'minute' : 'minutes' }}</span>
      </p>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted, onMounted } from 'vue'

const FOCUS_DURATION = 1;

const WORK_DURATION = FOCUS_DURATION * 60
const BREAK_DURATION = 5 * 60 // 5 minutes

const timeLeft = ref(WORK_DURATION)
const isBreak = ref(false)
const isRunning = ref(false)
let interval: number | undefined

const focusSessions = ref(0)
const minutes = computed(() => Math.floor(timeLeft.value / 60))
const seconds = computed(() => timeLeft.value % 60)

const focusMinutesTotal = computed(() => focusSessions.value * FOCUS_DURATION)
const focusHours = computed(() => Math.floor(focusMinutesTotal.value / 60))
const focusMinutes = computed(() => focusMinutesTotal.value % 60)

let originalTitle = document.title

function updateTitle() {
  const label = isBreak.value ? 'Break' : 'Focus'
  document.title = `${minutes.value}:${seconds.value < 10 ? '0' + seconds.value : seconds.value} - ${label} | TaskManager`
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
  timeLeft.value = isBreak.value ? BREAK_DURATION : WORK_DURATION
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
  }
  isBreak.value = !isBreak.value
  timeLeft.value = isBreak.value ? BREAK_DURATION : WORK_DURATION
  startTimer()
}

function setFocus() {
  pauseTimer()
  isBreak.value = false
  timeLeft.value = WORK_DURATION
}

function setBreak() {
  pauseTimer()
  isBreak.value = true
  timeLeft.value = BREAK_DURATION
}
</script>

<style scoped>
.pomodoro-container {
  max-width: 400px;
  margin: 3rem auto;
  padding: 2rem;
  background: var(--color-background-soft, #f8f8f8);
  border-radius: 16px;
  box-shadow: 0 4px 24px rgba(60,60,60,0.08);
  text-align: center;
}
.session-switch {
  display: flex;
  justify-content: center;
  gap: 1.2rem;
  margin-bottom: 1.5rem;
}
.session-switch button {
  background: #e0e0e0;
  color: #2c3e50;
  border: none;
  border-radius: 6px;
  padding: 0.5rem 1.2rem;
  font-size: 1.05rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s, color 0.2s;
}
.session-switch button.active {
  background: #42b983;
  color: #fff;
}
.timer-card {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 2px 8px rgba(60,60,60,0.10);
  padding: 2rem 1rem 1.5rem 1rem;
  margin-bottom: 1.5rem;
}
.timer-label {
  font-size: 1.1rem;
  color: #42b983;
  font-weight: 600;
  margin-bottom: 0.5rem;
}
.timer-display {
  font-size: 3.5rem;
  font-weight: bold;
  letter-spacing: 2px;
  margin-bottom: 1.2rem;
  color: #2c3e50;
}
.timer-controls {
  display: flex;
  justify-content: center;
  gap: 1rem;
}
.timer-controls button {
  background: #42b983;
  color: white;
  border: none;
  border-radius: 6px;
  padding: 0.6rem 1.4rem;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.2s;
}
.timer-controls button:hover {
  background: #36996b;
}
.pomodoro-info {
  margin-top: 1.5rem;
  color: var(--vt-c-text-light-2, #666);
  font-size: 1rem;
}
.focus-summary {
  margin-top: 0.7rem;
  font-weight: 600;
  color: #2c3e50;
}
</style> 