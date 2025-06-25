import api from './api'

export interface LoginPayload {
  email: string
  password: string
}

export interface RegisterPayload {
  name: string
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  user: {
    id: string
    name: string
    email: string
  }
}

export async function login(payload: LoginPayload): Promise<AuthResponse> {
  const { data } = await api.post('/users/login', payload)
  if (data?.data?.token) {
    localStorage.setItem('auth_token', data.data.token)
  }
  return data.data
}

export async function register(payload: RegisterPayload): Promise<AuthResponse> {
  const { data } = await api.post('/users', payload)
  // Optionally auto-login after registration if token is returned
  if (data?.data?.token) {
    localStorage.setItem('auth_token', data.data.token)
  }
  return data.data
}

export function logout() {
  localStorage.removeItem('auth_token')
}

export function getToken(): string | null {
  return localStorage.getItem('auth_token')
}

 