import { defineStore } from 'pinia'
import { ref } from 'vue'
import * as AuthService from '@/services/auth'

const USER_KEY = 'auth_user'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthService.AuthResponse['user'] | null>(getStoredUser())
  const token = ref<string | null>(AuthService.getToken())
  const loading = ref(false)
  const error = ref<string | null>(null)

  function getStoredUser() {
    const stored = localStorage.getItem(USER_KEY)
    return stored ? JSON.parse(stored) : null
  }

  async function login(payload: AuthService.LoginPayload) {
    loading.value = true
    error.value = null
    try {
      const res = await AuthService.login(payload)
      user.value = res.user
      token.value = res.token
      localStorage.setItem(USER_KEY, JSON.stringify(res.user))
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Login failed'
    } finally {
      loading.value = false
    }
  }

  async function register(payload: AuthService.RegisterPayload) {
    loading.value = true
    error.value = null
    try {
      await AuthService.register(payload)
      // Do not set user or token here; registration does not return them
    } catch (err: any) {
      error.value = err.response?.data?.message || 'Registration failed'
    } finally {
      loading.value = false
    }
  }

  function logout() {
    AuthService.logout()
    user.value = null
    token.value = null
    localStorage.removeItem(USER_KEY)
  }

  return { user, token, loading, error, login, register, logout }
}) 