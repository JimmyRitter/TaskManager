import { defineStore } from 'pinia'
import { ref } from 'vue'
import * as AuthService from '@/services/auth'

export const useAuthStore = defineStore('auth', () => {
  const user = ref<AuthService.AuthResponse['user'] | null>(null)
  const token = ref<string | null>(AuthService.getToken())
  const loading = ref(false)
  const error = ref<string | null>(null)

  async function login(payload: AuthService.LoginPayload) {
    loading.value = true
    error.value = null
    try {
      const res = await AuthService.login(payload)
      user.value = res.user
      token.value = res.token
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
      const res = await AuthService.register(payload)
      user.value = res.user
      token.value = res.token
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
  }

  return { user, token, loading, error, login, register, logout }
}) 