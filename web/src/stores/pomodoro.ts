import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const usePomodoroStore = defineStore('pomodoro', () => {
  const FOCUS_DURATION = 25 // 25 minutes
  const WORK_DURATION = FOCUS_DURATION * 60
  const BREAK_DURATION = 5 * 60 // 5 minutes

  // Timer state
  const timeLeft = ref(WORK_DURATION)
  const totalDuration = ref(WORK_DURATION)
  const isBreak = ref(false)
  const isRunning = ref(false)
  const focusSessions = ref(0)
  let interval: number | undefined

  // Computed values
  const minutes = computed(() => Math.floor(timeLeft.value / 60))
  const seconds = computed(() => timeLeft.value % 60)
  const progress = computed(() => {
    return (totalDuration.value - timeLeft.value) / totalDuration.value
  })
  const focusMinutesTotal = computed(() => focusSessions.value * FOCUS_DURATION)
  const focusHours = computed(() => Math.floor(focusMinutesTotal.value / 60))
  const focusMinutes = computed(() => focusMinutesTotal.value % 60)

  // Notification state
  const showNotification = ref(false)
  const notificationMessage = ref('')

  // Original document title for restoration
  let originalTitle = 'TaskManager'

  // Initialize original title
  function initializeTitle() {
    if (typeof document !== 'undefined') {
      originalTitle = document.title
    }
  }

  // Update browser title
  function updateTitle() {
    if (typeof document === 'undefined') return
    
    if (isRunning.value) {
      const label = isBreak.value ? 'Break' : 'Focus'
      const timeString = `${String(minutes.value).padStart(2, '0')}:${String(seconds.value).padStart(2, '0')}`
      document.title = `${timeString} - ${label} | TaskManager`
    } else {
      document.title = originalTitle
    }
  }

  // Show toast notification
  function showToast(message: string) {
    notificationMessage.value = message
    showNotification.value = true
    setTimeout(() => {
      showNotification.value = false
    }, 3000)
  }

  // Timer control functions
  function startTimer() {
    if (!isRunning.value) {
      isRunning.value = true
      interval = setInterval(() => {
        if (timeLeft.value > 0) {
          timeLeft.value--
          updateTitle()
        } else {
          switchSession()
        }
      }, 1000)
      updateTitle()
    }
  }

  function pauseTimer() {
    isRunning.value = false
    if (interval) {
      clearInterval(interval)
      interval = undefined
    }
    updateTitle()
  }

  function resetTimer() {
    pauseTimer()
    const duration = isBreak.value ? BREAK_DURATION : WORK_DURATION
    timeLeft.value = duration
    totalDuration.value = duration
    updateTitle()
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
    
    // Auto-start the next session after a brief delay
    setTimeout(() => {
      startTimer()
    }, 1000)
  }

  function setFocus() {
    pauseTimer()
    isBreak.value = false
    timeLeft.value = WORK_DURATION
    totalDuration.value = WORK_DURATION
    updateTitle()
  }

  function setBreak() {
    pauseTimer()
    isBreak.value = true
    timeLeft.value = BREAK_DURATION
    totalDuration.value = BREAK_DURATION
    updateTitle()
  }

  // Cleanup function
  function cleanup() {
    if (interval) {
      clearInterval(interval)
      interval = undefined
    }
    if (typeof document !== 'undefined') {
      document.title = originalTitle
    }
  }

  // Reset function for logout
  function reset() {
    cleanup()
    timeLeft.value = WORK_DURATION
    totalDuration.value = WORK_DURATION
    isBreak.value = false
    isRunning.value = false
    focusSessions.value = 0
    showNotification.value = false
    notificationMessage.value = ''
  }

  return {
    // State
    timeLeft,
    totalDuration,
    isBreak,
    isRunning,
    focusSessions,
    showNotification,
    notificationMessage,
    FOCUS_DURATION,
    
    // Computed
    minutes,
    seconds,
    progress,
    focusMinutesTotal,
    focusHours,
    focusMinutes,
    
    // Actions
    initializeTitle,
    startTimer,
    pauseTimer,
    resetTimer,
    toggleTimer,
    switchSession,
    setFocus,
    setBreak,
    showToast,
    cleanup,
    reset
  }
}) 