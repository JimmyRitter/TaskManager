<template>
  <div class="min-h-screen bg-gradient-to-br from-orange-50 to-orange-100 py-8">
    <div class="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
      <!-- Header Section -->
      <div class="text-center mb-8">
        <h1 class="text-4xl font-bold text-gray-900 mb-4">
          Change Password
        </h1>
        <p class="text-xl text-gray-600">
          Update your account password securely
        </p>
      </div>

      <!-- Password Update Section -->
      <div class="max-w-md mx-auto">
        <div class="bg-white rounded-2xl shadow-xl p-8">
          <h2 class="text-2xl font-semibold text-gray-900 mb-6">Change Password</h2>
            <form @submit.prevent="updatePassword" class="space-y-6">
              <div>
                <label for="currentPassword" class="block text-sm font-medium text-gray-700 mb-2">
                  Current Password
                </label>
                <input
                  id="currentPassword"
                  v-model="passwordForm.currentPassword"
                  type="password"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-colors"
                  placeholder="Enter current password"
                />
              </div>
              
              <div>
                <label for="newPassword" class="block text-sm font-medium text-gray-700 mb-2">
                  New Password
                </label>
                <input
                  id="newPassword"
                  v-model="passwordForm.newPassword"
                  type="password"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-colors"
                  placeholder="Enter new password"
                />
                <div class="mt-2">
                  <div class="text-xs text-gray-500 space-y-1">
                    <div :class="passwordValidation.length ? 'text-green-600' : 'text-gray-400'">
                      ✓ At least 8 characters
                    </div>
                    <div :class="passwordValidation.uppercase ? 'text-green-600' : 'text-gray-400'">
                      ✓ One uppercase letter
                    </div>
                    <div :class="passwordValidation.lowercase ? 'text-green-600' : 'text-gray-400'">
                      ✓ One lowercase letter
                    </div>
                    <div :class="passwordValidation.number ? 'text-green-600' : 'text-gray-400'">
                      ✓ One number
                    </div>
                  </div>
                </div>
              </div>
              
              <div>
                <label for="confirmPassword" class="block text-sm font-medium text-gray-700 mb-2">
                  Confirm New Password
                </label>
                <input
                  id="confirmPassword"
                  v-model="passwordForm.confirmPassword"
                  type="password"
                  class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-orange-500 focus:border-orange-500 transition-colors"
                  placeholder="Confirm new password"
                />
                <div v-if="passwordForm.confirmPassword && !passwordsMatch" class="mt-2 text-xs text-red-600">
                  Passwords do not match
                </div>
              </div>
              
              <button
                type="submit"
                :disabled="isUpdatingPassword || !canUpdatePassword"
                class="w-full bg-orange-500 hover:bg-orange-600 disabled:bg-orange-300 text-white font-semibold py-3 px-6 rounded-lg transition-colors"
              >
                {{ isUpdatingPassword ? 'Updating...' : 'Update Password' }}
              </button>
            </form>
          </div>
        </div>
      </div>
    </div>

    <!-- Success/Error Messages -->
    <div v-if="message" class="fixed bottom-4 right-4 z-50">
      <div 
        :class="[
          'px-6 py-4 rounded-lg shadow-lg text-white font-medium',
          messageType === 'success' ? 'bg-green-500' : 'bg-red-500'
        ]"
      >
        {{ message }}
      </div>
    </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { passwordService } from '@/services/password'

// Loading states
const isUpdatingPassword = ref(false)

// Message state
const message = ref('')
const messageType = ref<'success' | 'error'>('success')

// Password form
const passwordForm = ref({
  currentPassword: '',
  newPassword: '',
  confirmPassword: ''
})

// Password validation
const passwordValidation = computed(() => ({
  length: passwordForm.value.newPassword.length >= 8,
  uppercase: /[A-Z]/.test(passwordForm.value.newPassword),
  lowercase: /[a-z]/.test(passwordForm.value.newPassword),
  number: /\d/.test(passwordForm.value.newPassword)
}))

const passwordsMatch = computed(() => 
  passwordForm.value.newPassword === passwordForm.value.confirmPassword
)

const canUpdatePassword = computed(() => 
  passwordForm.value.currentPassword &&
  passwordForm.value.newPassword &&
  passwordForm.value.confirmPassword &&
  passwordsMatch.value &&
  Object.values(passwordValidation.value).every(Boolean)
)

const showMessage = (text: string, type: 'success' | 'error') => {
  message.value = text
  messageType.value = type
  setTimeout(() => {
    message.value = ''
  }, 5000)
}

const updatePassword = async () => {
  isUpdatingPassword.value = true
  try {
    const response = await passwordService.changePassword({
      currentPassword: passwordForm.value.currentPassword,
      newPassword: passwordForm.value.newPassword
    })
    
    if (response.success) {
      // Clear password form
      passwordForm.value = {
        currentPassword: '',
        newPassword: '',
        confirmPassword: ''
      }
      
      showMessage('Password updated successfully!', 'success')
    } else {
      showMessage(response.message || 'Failed to update password', 'error')
    }
  } catch (error: any) {
    console.error('Password change error:', error)
    showMessage(error.message || 'Failed to update password. Please try again.', 'error')
  } finally {
    isUpdatingPassword.value = false
  }
}
</script> 