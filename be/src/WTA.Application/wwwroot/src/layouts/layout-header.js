import html from '../utils/index.js';
import { reactive } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import SvgIcon from '../components/svg-icon.js';
import { useAppStore } from '../store/index.js';

import HeaderLogo from './header-logo.js';
import HeaderSettings from './header-settings.js';

export default {
  components: { HeaderLogo, HeaderSettings, SvgIcon, ElMessage, ElMessageBox },
  template: html` <div class="d-flex justify-content-between align-items-center h-100">
      <div class="d-flex justify-content-between align-items-center h-100">
        <header-logo :has-aside="hasAside" />
        <div
          v-if="hasAside"
          class="center"
        >
          <el-icon
            :size="18"
            class="cursor-pointer"
            @click="toggleMenu"
          >
            <ep-expand v-if="appStore.menuCollapse" />
            <ep-fold v-else />
          </el-icon>
        </div>
        <!-- <el-menu
      mode="horizontal"
      :default-active="$route.matched[0].path"
      :ellipsis="false"
      router
    >
      <template v-for="route in $router.options.routes">
        <el-menu-item
          v-if="!route.meta?.hide"
          :key="route.path"
          :index="route.path"
        >
          <template #title>
            <el-icon v-if="route.meta.icon">
              <svg-icon :name="route.meta.icon" />
            </el-icon>
            <span>{{ route.meta?.title ?? route.path }}</span>
          </template>
        </el-menu-item>
      </template>
    </el-menu> -->
      </div>
      <div class="d-flex justify-content-between align-items-center h-100">
        <el-space>
          <template v-if="appStore.user.isAuthenticated">
            <el-dropdown class="cursor-pointer">
              <el-space>
                <el-icon :size="18"
                  ><img
                    :src="userStore.avatar"
                    class="h-full"
                /></el-icon>
                <span>{{ appStore.user.name }}</span>
                <el-icon>
                  <i-ep-arrow-down />
                </el-icon>
              </el-space>
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item>
                    <a to="/account">
                      <el-icon> <i-ep-user /> </el-icon>{{$t('userCenter')}}
                    </a>
                  </el-dropdown-item>
                  <el-dropdown-item
                    divided
                    @click="confirmLogout"
                  >
                    <el-icon> <i-ep-switch-button /> </el-icon>退出登录
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
                  <i-ep-arrow-down />
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
            @click="setting.toggle()"
          >
            <ep-setting />
          </el-icon>
        </el-space>
      </div>
    </div>
    <el-drawer
      v-model="setting.show"
      title="页面配置"
      append-to-body
      destroy-on-close
      size="auto"
    >
      <header-settings />
    </el-drawer>`,
  props: {
    hasAside: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const appStore = useAppStore();

    const setting = reactive({
      show: false,
      toggle() {
        this.show = !this.show;
      },
    });

    const toggleMenu = () => (appStore.menuCollapse = !appStore.menuCollapse);
    // const onRoleChange = (command) => {
    //   userStore.currentRole = command;
    //   if (currentRoute.meta?.requiresAuth && !userStore.hasPermission(currentRoute.meta?.permission)) {
    //     router.push('/403');
    //   }
    // };

    const confirmLogout = async () => {
      try {
        await ElMessageBox.confirm('确认退出？', '提示', { type: 'warning' });
        window.location = appStore.basePath;
      } catch (error) {
        console.log(error);
        ElMessage({
          type: 'info',
          message: '退出取消',
        });
      }
    };
    return {
      setting,
      appStore,
      toggleMenu,
      confirmLogout,
    };
  },
};
