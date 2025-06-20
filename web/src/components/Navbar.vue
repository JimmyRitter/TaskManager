<template>
  <nav class="bg-white shadow-lg border-b border-gray-200 sticky top-0 z-50">
    <div class="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
      <div class="flex justify-between items-center h-16">
        <!-- Logo/Brand -->
        <div class="flex-shrink-0 flex items-center">
                  <RouterLink to="/" class="text-2xl font-bold text-gray-900 hover:text-orange-600 transition-colors duration-200">
          TaskManager
        </RouterLink>
        </div>

        <!-- Desktop Navigation -->
        <div class="hidden md:block">
          <div class="ml-10 flex items-baseline space-x-8">
            <RouterLink 
              v-if="isAuthenticated" 
              to="/dashboard" 
              class="text-gray-700 hover:text-orange-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200"
              :class="{ 'text-orange-600 bg-orange-50': $route.path === '/dashboard' }"
            >
              Dashboard
            </RouterLink>
            <RouterLink 
              v-if="isAuthenticated" 
              to="/pomodoro" 
              class="text-gray-700 hover:text-orange-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200"
              :class="{ 'text-orange-600 bg-orange-50': $route.path === '/pomodoro' }"
            >
              Pomodoro
            </RouterLink>
            <!-- <RouterLink 
              v-if="!isAuthenticated" 
              to="/about" 
              class="text-gray-700 hover:text-orange-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200"
              :class="{ 'text-orange-600 bg-orange-50': $route.path === '/about' }"
            >
              About
            </RouterLink> -->
          </div>
        </div>

        <!-- Desktop Auth Actions -->
        <div class="hidden md:block">
          <div class="ml-4 flex items-center md:ml-6">
                         <div v-if="isAuthenticated" class="relative">
              <!-- User dropdown -->
              <button
                @click="toggleDropdown"
                class="flex items-center text-sm rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-orange-500 transition-all duration-200"
                id="user-menu"
                aria-expanded="false"
                aria-haspopup="true"
              >
                <div class="h-8 w-8 rounded-full bg-orange-600 flex items-center justify-center text-white font-medium">
                  {{ auth.user?.name?.charAt(0) || 'U' }}
                </div>
                <span class="ml-2 text-gray-700 font-medium">{{ auth.user?.name || 'User' }}</span>
                <ChevronDownIcon 
                  class="ml-1 h-4 w-4 text-gray-500 transition-transform duration-200" 
                  :class="{ 'rotate-180': dropdownOpen }"
                />
              </button>

              <!-- Dropdown menu -->
              <Transition
                enter-active-class="transition ease-out duration-100"
                enter-from-class="transform opacity-0 scale-95"
                enter-to-class="transform opacity-100 scale-100"
                leave-active-class="transition ease-in duration-75"
                leave-from-class="transform opacity-100 scale-100"
                leave-to-class="transform opacity-0 scale-95"
              >
                <div 
                  v-show="dropdownOpen"
                  class="origin-top-right absolute right-0 mt-2 w-48 rounded-md shadow-lg py-1 bg-white ring-1 ring-black ring-opacity-5 focus:outline-none"
                  role="menu"
                  aria-orientation="vertical"
                  aria-labelledby="user-menu"
                >
                  <a href="#" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors duration-200" role="menuitem">
                    Profile
                  </a>
                  <a href="#" class="block px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors duration-200" role="menuitem">
                    Settings
                  </a>
                  <button 
                    @click="logout"
                    class="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100 transition-colors duration-200" 
                    role="menuitem"
                  >
                    Sign out
                  </button>
                </div>
              </Transition>
            </div>

            <!-- Login/Register buttons for non-authenticated users -->
            <div v-else class="flex items-center space-x-4">
              <RouterLink 
                to="/login" 
                class="text-gray-700 hover:text-orange-600 px-3 py-2 rounded-md text-sm font-medium transition-colors duration-200"
              >
                Sign in
              </RouterLink>
              <RouterLink 
                to="/register" 
                class="bg-orange-600 text-white hover:bg-orange-700 px-4 py-2 rounded-md text-sm font-medium transition-colors duration-200"
              >
                Sign up
              </RouterLink>
            </div>
          </div>
        </div>

        <!-- Mobile menu button -->
        <div class="md:hidden">
          <button
            @click="toggleMobileMenu"
            class="inline-flex items-center justify-center p-2 rounded-md text-gray-400 hover:text-gray-500 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-inset focus:ring-blue-500"
            aria-expanded="false"
          >
            <span class="sr-only">Open main menu</span>
            <Bars3Icon v-if="!mobileMenuOpen" class="block h-6 w-6" />
            <XMarkIcon v-else class="block h-6 w-6" />
          </button>
        </div>
      </div>
    </div>

    <!-- Mobile menu -->
    <Transition
      enter-active-class="transition ease-out duration-100"
      enter-from-class="transform opacity-0 scale-95"
      enter-to-class="transform opacity-100 scale-100"
      leave-active-class="transition ease-in duration-75"
      leave-from-class="transform opacity-100 scale-100"
      leave-to-class="transform opacity-0 scale-95"
    >
      <div v-show="mobileMenuOpen" class="md:hidden">
        <div class="px-2 pt-2 pb-3 space-y-1 sm:px-3 bg-white border-t border-gray-200">
                     <RouterLink 
             v-if="isAuthenticated" 
             to="/dashboard" 
             class="text-gray-700 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium transition-colors duration-200"
             :class="{ 'text-blue-600 bg-blue-50': $route.path === '/dashboard' }"
             @click="closeMobileMenu"
           >
             Dashboard
           </RouterLink>
           <RouterLink 
             v-if="isAuthenticated" 
             to="/pomodoro" 
             class="text-gray-700 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium transition-colors duration-200"
             :class="{ 'text-blue-600 bg-blue-50': $route.path === '/pomodoro' }"
             @click="closeMobileMenu"
           >
             Pomodoro
           </RouterLink>
           <RouterLink 
             v-if="!isAuthenticated" 
             to="/about" 
             class="text-gray-700 hover:text-blue-600 block px-3 py-2 rounded-md text-base font-medium transition-colors duration-200"
             :class="{ 'text-blue-600 bg-blue-50': $route.path === '/about' }"
             @click="closeMobileMenu"
           >
             About
           </RouterLink>
        </div>
        
        <!-- Mobile auth section -->
        <div class="pt-4 pb-3 border-t border-gray-200">
                     <div v-if="isAuthenticated" class="px-4">
            <div class="flex items-center">
              <div class="h-10 w-10 rounded-full bg-blue-600 flex items-center justify-center text-white font-medium">
                {{ auth.user?.name?.charAt(0) || 'U' }}
              </div>
              <div class="ml-3">
                <div class="text-base font-medium text-gray-800">{{ auth.user?.name || 'User' }}</div>
                <div class="text-sm font-medium text-gray-500">{{ auth.user?.email || 'user@example.com' }}</div>
              </div>
            </div>
            <div class="mt-3 space-y-1">
              <button 
                class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100 transition-colors duration-200 w-full text-left"
              >
                Profile
              </button>
              <button 
                class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100 transition-colors duration-200 w-full text-left"
              >
                Settings
              </button>
              <button 
                @click="logout"
                class="block px-4 py-2 text-base font-medium text-gray-500 hover:text-gray-800 hover:bg-gray-100 transition-colors duration-200 w-full text-left"
              >
                Sign out
              </button>
            </div>
          </div>
          <div v-else class="px-4 space-y-2">
            <RouterLink 
              to="/login" 
              class="block w-full text-center px-4 py-2 text-base font-medium text-gray-700 hover:text-blue-600 transition-colors duration-200"
              @click="closeMobileMenu"
            >
              Sign in
            </RouterLink>
            <RouterLink 
              to="/register" 
              class="block w-full text-center bg-blue-600 text-white hover:bg-blue-700 px-4 py-2 rounded-md text-base font-medium transition-colors duration-200"
              @click="closeMobileMenu"
            >
              Sign up
            </RouterLink>
          </div>
        </div>
      </div>
    </Transition>
  </nav>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import { useAuthStore } from '@/stores/auth'
import { ChevronDownIcon, Bars3Icon, XMarkIcon } from '@heroicons/vue/24/outline'

const auth = useAuthStore()
const router = useRouter()

const dropdownOpen = ref(false)
const mobileMenuOpen = ref(false)

// Computed property for authentication status
const isAuthenticated = computed(() => !!auth.user && !!auth.token)

function logout() {
  auth.logout()
  dropdownOpen.value = false
  mobileMenuOpen.value = false
  router.push('/login')
}

function toggleDropdown() {
  dropdownOpen.value = !dropdownOpen.value
}

function toggleMobileMenu() {
  mobileMenuOpen.value = !mobileMenuOpen.value
}

function closeMobileMenu() {
  mobileMenuOpen.value = false
}

function closeDropdown() {
  dropdownOpen.value = false
}

// Close dropdowns when clicking outside
function handleClickOutside(event: Event) {
  const target = event.target as Element
  if (!target.closest('#user-menu') && !target.closest('[role="menu"]')) {
    dropdownOpen.value = false
  }
  if (!target.closest('.md\\:hidden button') && !target.closest('.md\\:hidden div')) {
    mobileMenuOpen.value = false
  }
}

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script> 