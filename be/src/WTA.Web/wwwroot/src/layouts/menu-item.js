import html from '../utils/index.js';
import SvgIcon from '../components/svg-icon.js';
import Enumerable from '../../libs/linq/linq.min.js';
import { reactive } from 'vue';
import { useAppStore } from '../store/index.js';

const template = html`<template v-for="item in model">
  <template v-if="item.type!=='Action'">
    <el-sub-menu
      v-if="item.children?.filter(o=>o.type!=='Action').length"
      :key="item.id+'group'"
      :index="item.id+'group'"
    >
      <template #title>
        <el-icon><svg-icon :name="item.icon??'folder'" /></el-icon>
        <span style="margin-right: 25px">{{ item.name }}</span>
      </template>
      <menu-item
        v-model="item.children"
        :parentPath="parentPath?(parentPath+'/'+item.path):item.path"
      />
    </el-sub-menu>
    <el-menu-item
      v-else
      :key="item.id"
      :index="getUrl(item)"
      @click="redirect(item)"
    >
      <el-icon><svg-icon :name="item.icon??'page'" /></el-icon>
      <template #title>
        <span>{{ item.name }}</span>
      </template>
    </el-menu-item>
  </template>
</template> `;

export default {
  name: 'MenuItem', //
  components: { SvgIcon },
  template,
  props: {
    modelValue: {
      type: Object,
      default: null,
    },
    parentPath: {
      type: String,
      default: null,
    },
  },
  setup(props) {
    const appStore = useAppStore();
    const model = reactive(
      Enumerable.from(props.modelValue)
        .orderBy((o) => o.displayOrder)
        .toArray(),
    );
    const getUrl = (item) => {
      if (item.path === '/') {
        return item.path;
      }
      return (props.parentPath ? `${props.parentPath}/${item.path}` : item.path) + (item.redirect ? `/${item.redirect}` : '');
    };
    const redirect = (item) => {
      const url = getUrl(item);
      window.location.href = appStore.basePath === '/' ? url : `${appStore.basePath}${url}`;
    };
    return {
      model,
      getUrl,
      redirect,
    };
  },
};
