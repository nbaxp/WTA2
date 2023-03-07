import html from '../utils/index.js';
import { ref, watch } from 'vue';
import { useClipboard } from '@vueuse/core';
import { ElMessage } from 'element-plus';
import { useAppStore } from '../store/index.js';
import { Platform, Sunny, Moon } from '../../libs/@element-plus/icons-vue/index.min.js';

const template = html`<el-form
  ref="formRef"
  :model="appStore.settings"
  label-width="auto"
>
  <el-form-item
    prop="appStore.settings.mode"
    label="主题模式"
  >
    <el-select v-model="appStore.settings.mode">
      <template #prefix>
        <el-icon>
          <component :is="themeOptions.find((o) => o.value === appStore.settings.mode).icon" />
        </el-icon>
      </template>
      <template
        v-for="item in themeOptions"
        :key="item.value"
      >
        <el-option
          :value="item.value"
          :label="item.text"
        >
          <el-space>
            <el-icon>
              <component :is="item.icon" />
            </el-icon>
            <span>{{ item.text }}</span>
          </el-space>
        </el-option>
      </template>
    </el-select>
  </el-form-item>
  <el-form-item
    prop="appStore.settings.themeColor"
    label="主题色"
  >
    <el-color-picker v-model="appStore.settings.themeColor" />
  </el-form-item>
  <el-form-item
    prop="appStore.settings.size"
    label="组件大小"
  >
    <el-select v-model="appStore.settings.size">
      <template
        v-for="item in sizeOptions"
        :key="item.value"
      >
        <el-option
          :value="item.value"
          :label="item.text"
        >
          <span>{{ item.text }}</span>
        </el-option>
      </template>
    </el-select>
  </el-form-item>
  <el-form-item
    prop="appStore.settings.locale"
    label="默认语言"
  >
    <el-select v-model="appStore.settings.locale">
      <template
        v-for="item in localeOptions"
        :key="item.value"
      >
        <el-option
          :value="item.value"
          :label="item.text"
        >
          <span>{{ item.text }}</span>
        </el-option>
      </template>
    </el-select>
  </el-form-item>
  <el-form-item>
    <el-button @click="resetForm">恢复默认</el-button>
    <el-button
      type="primary"
      @click="copySettings"
      >复制配置</el-button
    >
  </el-form-item>
</el-form>`;

export default {
  components: { Platform, Sunny, Moon },
  template,
  setup() {
    const appStore = useAppStore();

    const formRef = ref(null);
    const resetForm = () => {
      formRef.value.resetFields();
      appStore.resetSettings();
    };

    const themeOptions = [
      { text: '跟随系统', value: 'auto', icon: Platform },
      { text: '亮色模式', value: 'light', icon: Sunny },
      { text: '暗色模式', value: 'dark', icon: Moon },
    ];

    const sizeOptions = [
      { text: '默认', value: 'default' },
      { text: '小', value: 'small' },
      { text: '大', value: 'large' },
    ];

    const localeOptions = [
      { text: '中文', value: 'zh' },
      { text: 'English', value: 'en' },
    ];

    const { copy } = useClipboard();

    const copySettings = async () => {
      const text = JSON.stringify(appStore.settings, null, 2);
      await copy(text);
      ElMessage({
        message: 'config/settings.json',
        type: 'success',
      });
    };

    return {
      appStore,
      formRef,
      resetForm,
      themeOptions,
      sizeOptions,
      localeOptions,
      copySettings,
    };
  },
};
