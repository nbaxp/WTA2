import html from '../utils/index.js';
import { useAppStore } from '../store/index.js';

export default {
  template: html` <div class="d-flex justify-content-center align-items-center h-100">
    {{ $t('copyright') }} v {{ appStore.version }}
  </div>`,
  setup() {
    return {
      appStore: useAppStore(),
    };
  },
};
