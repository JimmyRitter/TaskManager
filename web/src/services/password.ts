import api from './api'

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

export interface ChangePasswordResponse {
  message: string
  changedAt: string
}

export interface ApiResponse<T = any> {
  success: boolean
  data?: T
  message?: string
  errors?: string[]
}

export const passwordService = {
  async changePassword(request: ChangePasswordRequest): Promise<ApiResponse<ChangePasswordResponse>> {
    try {
      const response = await api.post('/users/change-password', request)
      return response.data
    } catch (error: any) {
      if (error.response?.data) {
        return error.response.data
      }
      throw new Error('Network error occurred')
    }
  }
} 