import { defineStore } from 'pinia'
import { ref, computed } from 'vue'

export const usePomodoroStore = defineStore('pomodoro', () => {
  // Configurable durations (in minutes for focus, seconds for work/break)
  const FOCUS_DURATION_MINUTES = ref(25) // Default 25 minutes (production)
  const BREAK_DURATION_MINUTES = ref(5) // Default 5 minutes (production)
  
  // Computed durations in seconds
  const WORK_DURATION = computed(() => FOCUS_DURATION_MINUTES.value * 60)
  const BREAK_DURATION = computed(() => BREAK_DURATION_MINUTES.value * 60)

  // Timer state
  const timeLeft = ref(WORK_DURATION.value)
  const totalDuration = ref(WORK_DURATION.value)
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
  const focusMinutesTotal = computed(() => focusSessions.value * FOCUS_DURATION_MINUTES.value)
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

  // Request browser notification permission
  async function requestNotificationPermission() {
    if ('Notification' in window) {
      const permission = await Notification.requestPermission()
      return permission === 'granted'
    }
    return false
  }

  // Show browser notification
  function showBrowserNotification(title: string, message: string, icon?: string) {
    if ('Notification' in window && Notification.permission === 'granted') {
      const notification = new Notification(title, {
        body: message,
        icon: icon || '/favicon.ico',
        badge: '/favicon.ico',
        requireInteraction: true, // Keeps notification visible until user interacts
        tag: 'pomodoro-timer' // Replaces previous notifications
      })
      
      // Auto-close after 10 seconds if user doesn't interact
      setTimeout(() => {
        notification.close()
      }, 10000)
    }
  }

  // Play audio alert
  function playAudioAlert(type: 'focus-end' | 'break-end' = 'focus-end') {
    try {
      // Create audio context for more reliable sound generation
      const audioContext = new (window.AudioContext || (window as any).webkitAudioContext)()
      
      // Different tones for different session types
      const frequencies = type === 'focus-end' 
        ? [800, 600, 400] // Descending tone for focus end (rest time)
        : [400, 600, 800] // Ascending tone for break end (back to work)
      
      frequencies.forEach((freq, index) => {
        setTimeout(() => {
          const oscillator = audioContext.createOscillator()
          const gainNode = audioContext.createGain()
          
          oscillator.connect(gainNode)
          gainNode.connect(audioContext.destination)
          
          oscillator.frequency.setValueAtTime(freq, audioContext.currentTime)
          oscillator.type = 'sine'
          
          gainNode.gain.setValueAtTime(0.3, audioContext.currentTime)
          gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.5)
          
          oscillator.start(audioContext.currentTime)
          oscillator.stop(audioContext.currentTime + 0.5)
        }, index * 200)
      })
    } catch (error) {
      console.warn('Could not play audio alert:', error)
    }
  }

  // Show alert when session ends
  function showSessionAlert(type: 'focus-end' | 'break-end') {
    const isBreakEnd = type === 'break-end'
    
    // Toast notification (always shown)
    const toastMessage = isBreakEnd 
      ? '✨ Break over! Ready for another focus session?'
      : '🎉 Work session complete! Time for a break.'
    showToast(toastMessage)
    
    // Browser notification
    const title = isBreakEnd ? 'Break Complete!' : 'Focus Session Complete!'
    const message = isBreakEnd 
      ? 'Time to get back to work. Ready for another focus session?'
      : 'Great job! Take a well-deserved break.'
    showBrowserNotification(title, message)
    
    // Audio alert
    playAudioAlert(type)
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
    const duration = isBreak.value ? BREAK_DURATION.value : WORK_DURATION.value
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
      showSessionAlert('focus-end')
      isBreak.value = true
      timeLeft.value = BREAK_DURATION.value
      totalDuration.value = BREAK_DURATION.value
    } else {
      showSessionAlert('break-end')
      isBreak.value = false
      timeLeft.value = WORK_DURATION.value
      totalDuration.value = WORK_DURATION.value
    }
    
    // Auto-start the next session after a brief delay
    setTimeout(() => {
      startTimer()
    }, 1000)
  }

  function setFocus() {
    pauseTimer()
    isBreak.value = false
    timeLeft.value = WORK_DURATION.value
    totalDuration.value = WORK_DURATION.value
    updateTitle()
  }

  function setBreak() {
    pauseTimer()
    isBreak.value = true
    timeLeft.value = BREAK_DURATION.value
    totalDuration.value = BREAK_DURATION.value
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
    timeLeft.value = WORK_DURATION.value
    totalDuration.value = WORK_DURATION.value
    isBreak.value = false
    isRunning.value = false
    focusSessions.value = 0
    showNotification.value = false
    notificationMessage.value = ''
  }

  // Functions to update durations for testing
  function setFocusDuration(minutes: number) {
    FOCUS_DURATION_MINUTES.value = minutes
    // If currently in focus mode, update the timer
    if (!isBreak.value) {
      timeLeft.value = WORK_DURATION.value
      totalDuration.value = WORK_DURATION.value
    }
  }

  function setBreakDuration(minutes: number) {
    BREAK_DURATION_MINUTES.value = minutes
    // If currently in break mode, update the timer
    if (isBreak.value) {
      timeLeft.value = BREAK_DURATION.value
      totalDuration.value = BREAK_DURATION.value
    }
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
    FOCUS_DURATION_MINUTES,
    BREAK_DURATION_MINUTES,
    
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
    showSessionAlert,
    requestNotificationPermission,
    cleanup,
    reset,
    setFocusDuration,
    setBreakDuration
  }
}) 