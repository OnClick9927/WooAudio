using UnityEditor;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using static WooAudio.AudioConfig;
namespace WooAudio
{
    [UnityEditor.CustomEditor(typeof(AudioConfig))]
    class AudioConfigEditor : Editor
    {
        AudioConfig cfg;
        private ReorderableList list;
        private Tree tree;
        private void OnEnable()
        {
            cfg = (AudioConfig)target;
            list = new ReorderableList(cfg.channels, typeof(string));
            list.drawHeaderCallback = (rect) =>
            {
                GUI.Label(rect, nameof(AudioConfig.channels));
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var src = cfg.channels[index];
                var tmp = EditorGUI.TextField(rect, src);
                if (tmp != src)
                {
                    cfg.channels[index] = tmp;
                }
            };
            tree = new Tree(new TreeViewState(), cfg);
        }
        private class Tree : TreeView
        {
            private AudioConfig cfg;

            public Tree(TreeViewState state, AudioConfig cfg) : base(state)
            {
                this.cfg = cfg;
                this.showAlternatingRowBackgrounds = true;
                this.multiColumnHeader = new MultiColumnHeader(new MultiColumnHeaderState(new MultiColumnHeaderState.Column[] {

                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.id))
                    },
                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.channel))
                    },
                        new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.clip))
                    },
                     new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.loop)),
                       width=35,
                       maxWidth=35,minWidth=35,

                    },
                         new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.cover)),
                           width=40,
                       maxWidth=40,minWidth=40,
                    },
                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.existTime))
                    },

                    new MultiColumnHeaderState.Column()
                    {
                       headerContent = new GUIContent(nameof(SoundData.volume))
                    },
                }));
                this.multiColumnHeader.ResizeToFit();
                Reload();
            }
            public bool change;
            protected override void RowGUI(RowGUIArgs args)
            {
                EditorGUI.BeginChangeCheck();
                var data = cfg._sounds[args.item.id];
                data.id = EditorGUI.IntField(args.GetCellRect(0), data.id);
                data.channel = EditorGUI.Popup(args.GetCellRect(1), Mathf.Clamp(data.channel, 0, cfg.channels.Count), cfg.channels.ToArray());
                var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(data.clip);
                var tmp = EditorGUI.ObjectField(args.GetCellRect(2), clip, typeof(AudioClip), false);
                if (tmp != clip)
                {
                    data.clip = AssetDatabase.GetAssetPath(tmp);
                }
                data.loop = EditorGUI.Toggle(args.GetCellRect(3), data.loop);
                data.cover = EditorGUI.Toggle(args.GetCellRect(4), data.cover);
                data.existTime = EditorGUI.IntField(args.GetCellRect(5), data.existTime);

                data.volume = EditorGUI.FloatField(args.GetCellRect(6), data.volume);
                if (EditorGUI.EndChangeCheck())
                {
                    change = true;
                }
            }
            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                IList<TreeViewItem> list = GetRows() ?? new List<TreeViewItem>();
                list.Clear();
                for (int i = 0; i < cfg._sounds.Count; i++)
                {
                    TreeViewItem item = new TreeViewItem()
                    {
                        id = i,
                        depth = 0,
                        displayName = i.ToString(),
                        parent = root,
                    };

                    list.Add(item);
                }
                return list;
            }
            protected override TreeViewItem BuildRoot() => new TreeViewItem() { depth = -1 };
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.UpdateIfRequiredOrScript();
            list.DoLayoutList();
            var rect = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true), GUILayout.MinHeight(400));
            tree.OnGUI(rect);
            if (tree.change)
            {
                this.serializedObject.ApplyModifiedProperties();

            }
        }



    }

}
