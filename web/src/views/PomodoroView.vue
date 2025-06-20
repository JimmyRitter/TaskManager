<template>
  <section class="pomodoro-container">
    <h1>Pomodoro Timer</h1>
    <div class="timer-card">
      <div class="timer-label">{{ isBreak ? 'Break' : 'Work Session' }}</div>
      <div class="timer-display">{{ minutes }}:{{ seconds < 10 ? '0' + seconds : seconds }}</div>
      <div class="timer-controls">
        <button @click="toggleTimer">{{ isRunning ? 'Pause' : 'Start' }}</button>
        <button @click="resetTimer">Reset</button>
      </div>
    </div>
    <div class="pomodoro-info">
      <p>Work for 25 minutes, then take a 5 minute break. Repeat to boost your productivity!</p>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, watch, onUnmounted } from 'vue'

const WORK_DURATION = 25 * 60 // 25 minutes
const BREAK_DURATION = 5 * 60 // 5 minutes

const timeLeft = ref(WORK_DURATION)
const isBreak = ref(false)
const isRunning = ref(false)
let interval: number | undefined

const minutes = computed(() => Math.floor(timeLeft.value / 60))
const seconds = computed(() => timeLeft.value % 60)

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
  isBreak.value = !isBreak.value
  timeLeft.value = isBreak.value ? BREAK_DURATION : WORK_DURATION
  startTimer()
}

watch(isBreak, () => {
  // Optionally, play a sound or show a notification
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})
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
</style> 