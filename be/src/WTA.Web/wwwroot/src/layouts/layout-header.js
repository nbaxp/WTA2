import html from '../utils/index.js';
import request from '../request/index.js';
import { ref, reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import SvgIcon from '../components/svg-icon.js';
import { useAppStore } from '../store/index.js';

import HeaderLogo from './header-logo.js';
import HeaderSettings from './header-settings.js';

const template = html` <div class="d-flex justify-content-between align-items-center h-100">
    <div class="d-flex justify-content-between align-items-center h-100">
      <header-logo :has-aside="hasAside" />
      <el-space
        class="h-100 center"
        v-if="hasAside"
      >
        <el-icon
          :size="18"
          class="cursor-pointer"
          @click="toggleMenu"
        >
          <svg-icon
            name="unfold"
            v-if="appStore.menuCollapse"
          />
          <svg-icon
            name="fold"
            v-else
          />
        </el-icon>
      </el-space>
    </div>
    <div class="d-flex justify-content-between align-items-center h-100">
      <el-space>
        <template v-if="appStore.user.isAuthenticated">
          <el-dropdown class="cursor-pointer">
            <el-space>
              <!-- <el-icon :size="18"
                ><img
                  :src="userStore.avatar"
                  class="h-full"
              /></el-icon> -->
              <span>{{ appStore.user.name }}</span>
              <el-icon>
                <ep-arrow-down />
              </el-icon>
            </el-space>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item>
                  <a to="/account">
                    <el-icon> <ep-user /> </el-icon>{{$t('userCenter')}}
                  </a>
                </el-dropdown-item>
                <el-dropdown-item
                  divided
                  @click="confirmLogout"
                >
                  <el-icon> <ep-switch-button /> </el-icon>退出登录
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
          <el-dropdown
            v-if="appStore.roleSwitchable && userStore.currentRole"
            class="cursor-pointer"
            @command="onRoleChange"
          >
            <el-space>
              <span>{{ userStore.roles.find((o) => o.number === userStore.currentRole)?.name }}</span>
              <el-icon>
                <ep-arrow-down />
              </el-icon>
            </el-space>
            <template #dropdown>
              <el-dropdown-menu>
                <el-dropdown-item
                  v-for="item in userStore.roles"
                  :key="item.number"
                  :command="item.number"
                  >{{ item.name }}
                </el-dropdown-item>
              </el-dropdown-menu>
            </template>
          </el-dropdown>
        </template>
        <template v-else>
          <el-space :size="20">
            <el-link type="info">
              <a
                class="router-link"
                :href="appStore.login"
              >
                {{$t('login')}}
              </a>
            </el-link>
            <el-link type="info">
              <a
                class="router-link"
                :href="appStore.register"
              >
                {{$t('register')}}</a
              >
            </el-link>
          </el-space>
        </template>
        <el-icon
          :size="18"
          class="cursor-pointer"
          @click="toggleSettings"
        >
          <ep-setting />
        </el-icon>
      </el-space>
    </div>
  </div>
  <el-drawer
    v-model="showSettings"
    title="页面配置"
    append-to-body
    destroy-on-close
    size="auto"
  >
    <header-settings />
  </el-drawer>`;

export default {
  components: { HeaderLogo, HeaderSettings, SvgIcon, ElMessage, ElMessageBox },
  template,
  props: {
    hasAside: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const appStore = useAppStore();

    const toggleMenu = () => (appStore.menuCollapse = !appStore.menuCollapse);

    const showSettings = ref(false);
    const toggleSettings = () => {
      showSettings.value = !showSettings.value;
    };

    const confirmLogout = async () => {
      const clientLogout = () => {
        localStorage.removeItem('token');
        window.location = appStore.basePath;
      };
      try {
        await ElMessageBox.confirm('确认退出？', '提示', { type: 'warning' });
        const config = { url: 'identity/account/logout', method: 'post' };
        const response = await request.request(config);
        clientLogout();
      } catch (error) {
        console.log(error);
        if (error === 'cancel') {
          ElMessage({
            type: 'info',
            message: '退出取消',
          });
        }
        else if (error.response?.status === 401) {
          clientLogout();
        }
      }
    };

    return {
      appStore,
      showSettings,
      toggleSettings,
      toggleMenu,
      confirmLogout,
    };
  },
};
