import { defineStore } from 'pinia';
import settings from '../config/settings.js';

const useAppStore = defineStore('app', {
  state: () => {
    return {
      ...settings,
    };
  },
});

export default useAppStore;
