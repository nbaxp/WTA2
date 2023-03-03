import html from '../utils/index.js';
import SvgIcon from '../components/svg-icon.js';
import Enumerable from '../../libs/linq/linq.min.js';
import { reactive } from 'vue';
import { useAppStore } from '../store/index.js';

const template = html`<template v-for="item in model">
  <el-sub-menu
    v-if="item.children.length"
    :key="item.id+'group'"
    :index="item.id+'group'"
  >
    <template #title>
      <el-icon><svg-icon :name="item.icon??'folder'" /></el-icon>
      <span style="margin-right: 25px">{{ item.name }}</span>
    </template>
    <menu-item v-model="item.children" />
  </el-sub-menu>
  <el-menu-item
    v-else
    :key="item.id"
    :index="item.url"
    @click="redirect(item.url)"
  >
    <el-icon><svg-icon :name="item.icon??'page'" /></el-icon>
    <template #title>
      <span>{{ item.name }}</span>
    </template>
  </el-menu-item>
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
  },
  setup(props) {
    const appStore = useAppStore();
    const model = reactive(
      Enumerable.from(props.modelValue)
        .orderBy((o) => o.displayOrder)
        .toArray(),
    );
    const redirect = (o) => {
      window.location.href = appStore.basePath === '/' ? o : `${appStore}${o}`;
    };
    return {
      model,
      redirect,
    };
  },
};
