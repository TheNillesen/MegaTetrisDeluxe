using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class GameObject
    {
        private Transform transform;
        private Spriterendere renderer;

        public Transform Transform { get { return transform; } }
        public Spriterendere Rendere { get { return renderer; } }

        private List<Component> components;

        public GameObject()
        {
            this.components = new List<Component>();
        }

        public GameObject(params Component[] components)
        {
            this.components = new List<Component>(components);
        }

        public void AddComponent(Component component)
        {
            components.Add(component);
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
        }

        public void RemoveAllComponent<T>() where T : Component
        {
            components.RemoveAll(o => o is T);
        }

        public T GetComponent<T>() where T : Component
        {
            return components.Find(o => o is T) as T;
        }

        public Component GetComponent(Predicate<Component> filter)
        {
            return components.Find(filter);
        }

        public T GetComponent<T>(Predicate<T> filter) where T : Component
        {
            return components.Find(o => o is T && filter(o as T)) as T;
        }
        
        public T[] GetComponents<T>() where T : Component
        {
            return (from Component comp in components where comp is T select comp as T).ToArray();
        }

        public T[] GetComponents<T>(Predicate<T> filter) where T : Component
        {
            return (from Component comp in components where comp is T && filter(comp as T) select comp as T).ToArray();
        }

        public Component[] Getcomponents(Predicate<Component> filter)
        {
            return components.FindAll(filter).ToArray();
        }

        public void Update()
        {
            
        }
    }
}
